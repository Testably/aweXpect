using System;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core.Helpers;
using aweXpect.Core.Internal;

namespace aweXpect.Core.Sources;

internal class DelegateAsyncValueSource<TValue>(Func<CancellationToken, Task<TValue>>? action)
	: IValueSource<DelegateValue<TValue>>
{
	#region IValueSource<DelegateValue<TValue>> Members

	public bool IsNullTaskSubject => false;

	public async ValueTask<DelegateValue<TValue>> GetValue(ITimeSystem timeSystem,
		CancellationToken cancellationToken)
	{
		if (action is null)
		{
			return new DelegateValue<TValue>(default, null, TimeSpan.Zero, true);
		}

		long startTimestamp = timeSystem.GetTimestamp();
		Task<TValue>? task = null;
		try
		{
			task = action(cancellationToken);
			if (task is null)
			{
				return new DelegateValue<TValue>(default, null, timeSystem.GetElapsedTime(startTimestamp))
				{
					NullKind = NullSubjectKind.NullTaskReturned,
				};
			}

			await task.WaitWithoutThrowing(cancellationToken);
			if (task.IsFaulted)
			{
				return new DelegateValue<TValue>(default, task.GetFirstException(out Exception[]? otherExceptions),
					timeSystem.GetElapsedTime(startTimestamp))
				{
					OtherExceptions = otherExceptions,
				};
			}

			TValue value = await task;
			return new DelegateValue<TValue>(value, null, timeSystem.GetElapsedTime(startTimestamp));
		}
		catch (Exception ex)
		{
			return new DelegateValue<TValue>(default, ex, timeSystem.GetElapsedTime(startTimestamp))
			{
				OtherExceptions = task?.GetOtherExceptions(ex),
			};
		}
	}

	public Exception[]? GetOtherExceptions(Exception exception) => null;

	#endregion
}
