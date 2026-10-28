using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
#if NET8_0_OR_GREATER
using aweXpect.Core.Helpers;
using aweXpect.Options;
#endif

namespace aweXpect.Core.Sources;

internal class DelegateSource : IValueSource<DelegateValue>
#if NET8_0_OR_GREATER
	, IAllocationMeasuringSource
#endif
{
	/// <remarks>
	///     Reading the custom attributes is costly compared to a delegate that does nothing, and the same lambda is
	///     passed on every call of a test. The table does not keep a collectible assembly alive.
	/// </remarks>
	private static readonly ConditionalWeakTable<MethodInfo, object> IsAsyncVoidMethod = new();

	private readonly Action<CancellationToken>? _action;
	private readonly Action? _actionWithoutCancellation;

	public DelegateSource(Action<CancellationToken>? action)
	{
		ThrowIfAsyncVoid(action?.GetMethodInfo(), "Func<CancellationToken, Task>");
		_action = action;
	}

	/// <remarks>
	///     Keeps the <paramref name="action" /> as it is, instead of wrapping it in a closure that ignores the token.
	/// </remarks>
	public DelegateSource(Action? action)
	{
		ThrowIfAsyncVoid(action?.GetMethodInfo(), "Func<Task>");
		_actionWithoutCancellation = action;
	}

	private static void ThrowIfAsyncVoid(MethodInfo? method, string replaceType)
	{
		if (method is not null && (bool)IsAsyncVoidMethod.GetValue(method,
			    static method => Attribute.IsDefined(method, typeof(AsyncStateMachineAttribute), true)))
		{
			throw Tracing.WriteException(
				new InvalidOperationException(
					$"Cannot use aweXpect on an async void method: use {replaceType} instead."));
		}
	}

	#region IValueSource<DelegateValue> Members

	public bool IsNullTaskSubject => false;

	public ValueTask<DelegateValue> GetValue(TimeProvider timeProvider,
		CancellationToken cancellationToken)
	{
		if (_action is null && _actionWithoutCancellation is null)
		{
			return new ValueTask<DelegateValue>(new DelegateValue(null, TimeSpan.Zero, true));
		}

#if NET8_0_OR_GREATER
		if (_allocationOptions is not null)
		{
			return new ValueTask<DelegateValue>(
				InvokeMeasuringAllocations(_allocationOptions, timeProvider, cancellationToken));
		}
#endif

		long startTimestamp = timeProvider.GetTimestamp();
		try
		{
			if (_action is null)
			{
				_actionWithoutCancellation!();
			}
			else
			{
				_action(cancellationToken);
			}

			return new ValueTask<DelegateValue>(new DelegateValue(null, timeProvider.GetElapsedTime(startTimestamp)));
		}
		catch (Exception ex)
		{
			return new ValueTask<DelegateValue>(new DelegateValue(ex, timeProvider.GetElapsedTime(startTimestamp)));
		}
	}

	public Exception[]? GetOtherExceptions(Exception exception) => null;

	#endregion

#if NET8_0_OR_GREATER
	private AllocationOptions? _allocationOptions;

	/// <inheritdoc cref="IAllocationMeasuringSource.MeasureAllocations(AllocationOptions)" />
	public void MeasureAllocations(AllocationOptions options)
	{
		ThrowHelper.ThrowIfAlreadySpecified(_allocationOptions is not null, "Allocates");
		_allocationOptions = options;
	}

	/// <remarks>
	///     The allocated bytes are read directly around the invocation, so that only the delegate is measured.
	/// </remarks>
	private DelegateValue InvokeMeasuringAllocations(AllocationOptions options, TimeProvider timeProvider,
		CancellationToken cancellationToken)
	{
		if (options.IsWarmingUp)
		{
			long warmUpTimestamp = timeProvider.GetTimestamp();
			try
			{
				Invoke(cancellationToken);
			}
			catch (Exception ex)
			{
				if (!options.AreExceptionsAllowed || cancellationToken.IsCancellationRequested)
				{
					return new DelegateValue(ex, timeProvider.GetElapsedTime(warmUpTimestamp))
					{
						IsWarmUpException = true,
					};
				}
			}
		}

		long startTimestamp = timeProvider.GetTimestamp();
		long allocatedBytesBefore = GC.GetAllocatedBytesForCurrentThread();
		try
		{
			Invoke(cancellationToken);
			long allocatedBytes = GC.GetAllocatedBytesForCurrentThread() - allocatedBytesBefore;
			return new DelegateValue(null, timeProvider.GetElapsedTime(startTimestamp))
			{
				AllocatedBytes = allocatedBytes,
			};
		}
		catch (Exception ex)
		{
			long allocatedBytes = GC.GetAllocatedBytesForCurrentThread() - allocatedBytesBefore;
			return new DelegateValue(ex, timeProvider.GetElapsedTime(startTimestamp))
			{
				AllocatedBytes = allocatedBytes,
			};
		}
	}

	private void Invoke(CancellationToken cancellationToken)
	{
		if (_action is null)
		{
			_actionWithoutCancellation!();
		}
		else
		{
			_action(cancellationToken);
		}
	}
#endif
}
