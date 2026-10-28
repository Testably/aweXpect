using System;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core.Helpers;

namespace aweXpect.Core.Sources;

internal class DelegateAsyncValueSource<TValue>(Func<CancellationToken, Task<TValue>>? action)
	: IValueSource<DelegateValue<TValue>>
{
	#region IValueSource<DelegateValue<TValue>> Members

	public bool IsNullTaskSubject => false;

	public async ValueTask<DelegateValue<TValue>> GetValue(TimeProvider timeProvider,
		CancellationToken cancellationToken)
	{
		if (action is null)
		{
			return new DelegateValue<TValue>(default, null, TimeSpan.Zero, true);
		}

		long startTimestamp = timeProvider.GetTimestamp();
		Task<TValue>? task = null;
		try
		{
			task = action(cancellationToken);
			if (task is null)
			{
				return new DelegateValue<TValue>(default, null, timeProvider.GetElapsedTime(startTimestamp))
				{
					NullKind = NullSubjectKind.NullTaskReturned,
				};
			}

			await task.WaitWithoutThrowing(cancellationToken);
			if (task.IsFaulted)
			{
				return new DelegateValue<TValue>(default, task.GetFirstException(out Exception[]? otherExceptions),
					timeProvider.GetElapsedTime(startTimestamp))
				{
					OtherExceptions = otherExceptions,
				};
			}

			TValue value = await task;
			return new DelegateValue<TValue>(value, null, timeProvider.GetElapsedTime(startTimestamp));
		}
		catch (Exception ex)
		{
			return new DelegateValue<TValue>(default, ex, timeProvider.GetElapsedTime(startTimestamp))
			{
				OtherExceptions = task?.GetOtherExceptions(ex),
			};
		}
	}

	public Exception[]? GetOtherExceptions(Exception exception) => null;

	#endregion
}
