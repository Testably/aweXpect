using System;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core.Helpers;
using aweXpect.Core.Internal;

namespace aweXpect.Core.Sources;

/// <remarks>
///     A <see cref="Task" /> subject is wrapped in the <paramref name="action" /> with <paramref name="isTaskSubject" />
///     set, so that a <see langword="null" /> task is named as a task instead of as a delegate.
/// </remarks>
internal class DelegateAsyncSource(Func<CancellationToken, Task>? action, bool isTaskSubject = false)
	: IValueSource<DelegateValue>
{
	#region IValueSource<DelegateValue> Members

	public bool IsNullTaskSubject => false;

	public async ValueTask<DelegateValue> GetValue(ITimeSystem timeSystem,
		CancellationToken cancellationToken)
	{
		if (action is null)
		{
			return new DelegateValue(null, TimeSpan.Zero)
			{
				NullKind = isTaskSubject ? NullSubjectKind.NullTaskSubject : NullSubjectKind.NullDelegate,
			};
		}

		long startTimestamp = timeSystem.GetTimestamp();
		Task? task = null;
		try
		{
			task = action(cancellationToken);
			if (task is null)
			{
				return new DelegateValue(null, timeSystem.GetElapsedTime(startTimestamp))
				{
					NullKind = NullSubjectKind.NullTaskReturned,
				};
			}

			await task.AbandonOnCancellation(cancellationToken);
			return new DelegateValue(null, timeSystem.GetElapsedTime(startTimestamp));
		}
		catch (Exception ex)
		{
			return new DelegateValue(ex, timeSystem.GetElapsedTime(startTimestamp))
			{
				OtherExceptions = task?.GetOtherExceptions(ex),
			};
		}
	}

	public Exception[]? GetOtherExceptions(Exception exception) => null;

	#endregion
}
