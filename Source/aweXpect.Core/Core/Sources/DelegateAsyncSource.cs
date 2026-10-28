using System;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core.Helpers;

namespace aweXpect.Core.Sources;

/// <remarks>
///     A <see cref="Task" /> subject is wrapped in the <paramref name="action" /> with <paramref name="isTaskSubject" />
///     set, so that a <see langword="null" /> task is named as a task instead of as a delegate.
/// </remarks>
internal class DelegateAsyncSource(Func<CancellationToken, Task>? action, bool isTaskSubject = false)
	: IValueSource<DelegateValue>
{
	private readonly Func<Task>? _actionWithoutCancellation;

	/// <remarks>
	///     Keeps the <paramref name="action" /> as it is, instead of wrapping it in a closure that ignores the token.
	/// </remarks>
	public DelegateAsyncSource(Func<Task>? action) : this(null, false)
	{
		_actionWithoutCancellation = action;
	}

	#region IValueSource<DelegateValue> Members

	public bool IsNullTaskSubject => false;

	public async ValueTask<DelegateValue> GetValue(TimeProvider timeProvider,
		CancellationToken cancellationToken)
	{
		if (action is null && _actionWithoutCancellation is null)
		{
			return new DelegateValue(null, TimeSpan.Zero)
			{
				NullKind = isTaskSubject ? NullSubjectKind.NullTaskSubject : NullSubjectKind.NullDelegate,
			};
		}

		long startTimestamp = timeProvider.GetTimestamp();
		Task? task = null;
		try
		{
			task = action is null ? _actionWithoutCancellation!() : action(cancellationToken);
			if (task is null)
			{
				return new DelegateValue(null, timeProvider.GetElapsedTime(startTimestamp))
				{
					NullKind = NullSubjectKind.NullTaskReturned,
				};
			}

			await task.WaitWithoutThrowing(cancellationToken);
			if (task.IsFaulted)
			{
				return new DelegateValue(task.GetFirstException(out Exception[]? otherExceptions),
					timeProvider.GetElapsedTime(startTimestamp))
				{
					OtherExceptions = otherExceptions,
				};
			}

			await task;
			return new DelegateValue(null, timeProvider.GetElapsedTime(startTimestamp));
		}
		catch (Exception ex)
		{
			return new DelegateValue(ex, timeProvider.GetElapsedTime(startTimestamp))
			{
				OtherExceptions = task?.GetOtherExceptions(ex),
			};
		}
	}

	public Exception[]? GetOtherExceptions(Exception exception) => null;

	#endregion
}
