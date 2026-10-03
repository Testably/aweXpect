using System;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core.Helpers;
using aweXpect.Core.TimeSystem;

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

		IStopwatch sw = timeSystem.Stopwatch.New();
		Task? task = null;
		try
		{
			sw.Start();
			task = action(cancellationToken);
			if (task is null)
			{
				return new DelegateValue(null, sw.Elapsed)
				{
					NullKind = NullSubjectKind.NullTaskReturned,
				};
			}

			await task.AbandonOnCancellation(cancellationToken);
			sw.Stop();
			return new DelegateValue(null, sw.Elapsed);
		}
		catch (Exception ex)
		{
			return new DelegateValue(ex, sw.Elapsed)
			{
				OtherExceptions = task?.GetOtherExceptions(ex),
			};
		}
	}

	public Exception[]? GetOtherExceptions(Exception exception) => null;

	#endregion
}
