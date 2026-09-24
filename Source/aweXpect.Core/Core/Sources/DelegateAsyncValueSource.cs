using System;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core.Helpers;
using aweXpect.Core.TimeSystem;

namespace aweXpect.Core.Sources;

internal class DelegateAsyncValueSource<TValue>(Func<CancellationToken, Task<TValue>>? action)
	: IValueSource<DelegateValue<TValue>>
{
	#region IValueSource<DelegateValue<TValue>> Members

	public async Task<DelegateValue<TValue>> GetValue(ITimeSystem timeSystem,
		CancellationToken cancellationToken)
	{
		if (action is null)
		{
			return new DelegateValue<TValue>(default, null, TimeSpan.Zero, true);
		}

		IStopwatch sw = timeSystem.Stopwatch.New();
		try
		{
			sw.Start();
			TValue value = await action(cancellationToken).AbandonOnCancellation(cancellationToken);
			sw.Stop();
			return new DelegateValue<TValue>(value, null, sw.Elapsed);
		}
		catch (Exception ex)
		{
			return new DelegateValue<TValue>(default, ex, sw.Elapsed);
		}
	}

	#endregion
}
