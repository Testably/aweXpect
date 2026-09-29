using System;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core.Helpers;
using aweXpect.Core.TimeSystem;

namespace aweXpect.Core.Sources;

internal class DelegateAsyncSource(Func<CancellationToken, Task>? action)
	: IValueSource<DelegateValue>
{
	#region IValueSource<DelegateValue> Members

	public async Task<DelegateValue> GetValue(ITimeSystem timeSystem,
		CancellationToken cancellationToken)
	{
		if (action is null)
		{
			return new DelegateValue(null, TimeSpan.Zero, true);
		}

		IStopwatch sw = timeSystem.Stopwatch.New();
		try
		{
			sw.Start();
			Task? task = action(cancellationToken);
			if (task is null)
			{
				return new DelegateValue(null, sw.Elapsed, true)
				{
					IsNullTask = true,
				};
			}

			await task.AbandonOnCancellation(cancellationToken);
			sw.Stop();
			return new DelegateValue(null, sw.Elapsed);
		}
		catch (Exception ex)
		{
			return new DelegateValue(ex, sw.Elapsed);
		}
	}

	#endregion
}
