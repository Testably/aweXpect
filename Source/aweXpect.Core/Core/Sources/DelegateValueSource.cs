using System;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core.TimeSystem;

namespace aweXpect.Core.Sources;

internal class DelegateValueSource<TValue> : IValueSource<DelegateValue<TValue>>
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

	public ValueTask<DelegateValue<TValue>> GetValue(ITimeSystem timeSystem,
		CancellationToken cancellationToken)
	{
		if (_action is null && _actionWithoutCancellation is null)
		{
			return new ValueTask<DelegateValue<TValue>>(new DelegateValue<TValue>(default, null, TimeSpan.Zero, true));
		}

		IStopwatch sw = timeSystem.Stopwatch.New();
		try
		{
			sw.Start();
			TValue value = _action is null ? _actionWithoutCancellation!() : _action(cancellationToken);
			sw.Stop();
			return new ValueTask<DelegateValue<TValue>>(new DelegateValue<TValue>(value, null, sw.Elapsed));
		}
		catch (Exception ex)
		{
			return new ValueTask<DelegateValue<TValue>>(new DelegateValue<TValue>(default, ex, sw.Elapsed));
		}
	}

	public Exception[]? GetOtherExceptions(Exception exception) => null;

	#endregion
}
