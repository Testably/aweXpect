using System;
using System.Threading;
using System.Threading.Tasks;
#if NET8_0_OR_GREATER
using aweXpect.Core.Helpers;
using aweXpect.Options;
#endif

namespace aweXpect.Core.Sources;

internal class DelegateValueSource<TValue> : IValueSource<DelegateValue<TValue>>
#if NET8_0_OR_GREATER
	, IAllocationMeasuringSource
#endif
{
	private readonly Func<CancellationToken, TValue>? _action;
	private readonly Func<TValue>? _actionWithoutCancellation;

	public DelegateValueSource(Func<CancellationToken, TValue>? action)
	{
		_action = action;
	}

	/// <remarks>
	///     Keeps the <paramref name="action" /> as it is, instead of wrapping it in a closure that ignores the token.
	/// </remarks>
	public DelegateValueSource(Func<TValue>? action)
	{
		_actionWithoutCancellation = action;
	}

	#region IValueSource<DelegateValue<TValue>> Members

	public bool IsNullTaskSubject => false;

	public ValueTask<DelegateValue<TValue>> GetValue(TimeProvider timeProvider,
		CancellationToken cancellationToken)
	{
		if (_action is null && _actionWithoutCancellation is null)
		{
			return new ValueTask<DelegateValue<TValue>>(new DelegateValue<TValue>(default, null, TimeSpan.Zero, true));
		}

#if NET8_0_OR_GREATER
		if (_allocationOptions is not null)
		{
			return new ValueTask<DelegateValue<TValue>>(
				InvokeMeasuringAllocations(_allocationOptions, timeProvider, cancellationToken));
		}
#endif

		long startTimestamp = timeProvider.GetTimestamp();
		try
		{
			TValue value = _action is null ? _actionWithoutCancellation!() : _action(cancellationToken);
			return new ValueTask<DelegateValue<TValue>>(new DelegateValue<TValue>(value, null,
				timeProvider.GetElapsedTime(startTimestamp)));
		}
		catch (Exception ex)
		{
			return new ValueTask<DelegateValue<TValue>>(new DelegateValue<TValue>(default, ex,
				timeProvider.GetElapsedTime(startTimestamp)));
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
	private DelegateValue<TValue> InvokeMeasuringAllocations(AllocationOptions options, TimeProvider timeProvider,
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
					return new DelegateValue<TValue>(default, ex, timeProvider.GetElapsedTime(warmUpTimestamp))
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
			TValue value = Invoke(cancellationToken);
			long allocatedBytes = GC.GetAllocatedBytesForCurrentThread() - allocatedBytesBefore;
			return new DelegateValue<TValue>(value, null, timeProvider.GetElapsedTime(startTimestamp))
			{
				AllocatedBytes = allocatedBytes,
			};
		}
		catch (Exception ex)
		{
			long allocatedBytes = GC.GetAllocatedBytesForCurrentThread() - allocatedBytesBefore;
			return new DelegateValue<TValue>(default, ex, timeProvider.GetElapsedTime(startTimestamp))
			{
				AllocatedBytes = allocatedBytes,
			};
		}
	}

	private TValue Invoke(CancellationToken cancellationToken)
		=> _action is null ? _actionWithoutCancellation!() : _action(cancellationToken);
#endif
}
