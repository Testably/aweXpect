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
		Task<TValue>? task = null;
		try
		{
			sw.Start();
			task = action(cancellationToken);
			if (task is null)
			{
				return new DelegateValue<TValue>(default, null, sw.Elapsed, true)
				{
					IsNullTask = true,
				};
			}

			TValue value = await task.AbandonOnCancellation(cancellationToken);
			sw.Stop();
			return new DelegateValue<TValue>(value, null, sw.Elapsed);
		}
		catch (Exception ex)
		{
			return new DelegateValue<TValue>(default, ex, sw.Elapsed)
			{
				OtherExceptions = task?.GetOtherExceptions(ex),
			};
		}
	}

	#endregion
}
