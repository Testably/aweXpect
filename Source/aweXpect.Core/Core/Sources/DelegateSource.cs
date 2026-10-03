using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core.TimeSystem;

namespace aweXpect.Core.Sources;

internal class DelegateSource : IValueSource<DelegateValue>
{
	/// <remarks>
	///     Reading the custom attributes is costly compared to a delegate that does nothing, and the same lambda is
	///     passed on every call of a test. The table does not keep a collectible assembly alive.
	/// </remarks>
	private static readonly ConditionalWeakTable<MethodInfo, object> IsAsyncVoidMethod = new();

	private readonly Action<CancellationToken>? _action;

	public DelegateSource(Action<CancellationToken>? action)
	{
		ThrowIfAsyncVoid(action?.GetMethodInfo(), "Func<CancellationToken, Task>");
		_action = action;
	}

	public DelegateSource(Action? action)
	{
		ThrowIfAsyncVoid(action?.GetMethodInfo(), "Func<Task>");
		_action = action is null ? null : _ => action();
	}

	#region IValueSource<DelegateValue> Members

	public bool IsNullTaskSubject => false;

	public ValueTask<DelegateValue> GetValue(ITimeSystem timeSystem,
		CancellationToken cancellationToken)
	{
		if (_action is null)
		{
			return new ValueTask<DelegateValue>(new DelegateValue(null, TimeSpan.Zero, true));
		}

		IStopwatch sw = timeSystem.Stopwatch.New();
		try
		{
			sw.Start();
			_action(cancellationToken);
			sw.Stop();
			return new ValueTask<DelegateValue>(new DelegateValue(null, sw.Elapsed));
		}
		catch (Exception ex)
		{
			return new ValueTask<DelegateValue>(new DelegateValue(ex, sw.Elapsed));
		}
	}

	public Exception[]? GetOtherExceptions(Exception exception) => null;

	#endregion

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
}
