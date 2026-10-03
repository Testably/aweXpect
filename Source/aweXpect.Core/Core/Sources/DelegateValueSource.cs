using System;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core.TimeSystem;

namespace aweXpect.Core.Sources;

internal class DelegateValueSource<TValue>(Func<CancellationToken, TValue>? action)
	: IValueSource<DelegateValue<TValue>>
{
	#region IValueSource<DelegateValue<TValue>> Members

	public ValueTask<DelegateValue<TValue>> GetValue(ITimeSystem timeSystem,
		CancellationToken cancellationToken)
	{
		if (action is null)
		{
			return new ValueTask<DelegateValue<TValue>>(new DelegateValue<TValue>(default, null, TimeSpan.Zero, true));
		}

		IStopwatch sw = timeSystem.Stopwatch.New();
		try
		{
			sw.Start();
			TValue value = action(cancellationToken);
			sw.Stop();
			return new ValueTask<DelegateValue<TValue>>(new DelegateValue<TValue>(value, null, sw.Elapsed));
		}
		catch (Exception ex)
		{
			return new ValueTask<DelegateValue<TValue>>(new DelegateValue<TValue>(default, ex, sw.Elapsed));
		}
	}

	#endregion
}
