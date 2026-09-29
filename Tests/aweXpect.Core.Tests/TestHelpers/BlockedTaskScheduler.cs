using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Threading;

namespace aweXpect.Core.Tests.TestHelpers;

/// <summary>
///     A single-threaded task scheduler that never runs the queued tasks, like a scheduler whose only thread is blocked.
/// </summary>
internal sealed class BlockedTaskScheduler : TaskScheduler
{
	private Thread? _thread;

	protected override IEnumerable<Task> GetScheduledTasks() => [];

	protected override void QueueTask(Task task) { }

	protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
		=> !taskWasPreviouslyQueued && Thread.CurrentThread == _thread && TryExecuteTask(task);

	/// <summary>
	///     Runs the <paramref name="action" /> on a dedicated thread as a task of a <see cref="BlockedTaskScheduler" />
	///     and returns whether it completed.
	/// </summary>
	/// <remarks>
	///     It gives up after ten seconds, so that a deadlock fails the test instead of hanging the test run.
	/// </remarks>
	public static bool Run(Action action)
	{
		BlockedTaskScheduler scheduler = new();
		ExceptionDispatchInfo? exception = null;
		scheduler._thread = new Thread(() =>
		{
			try
			{
				Task task = new(action);
				task.RunSynchronously(scheduler);
				task.GetAwaiter().GetResult();
			}
			catch (Exception ex)
			{
				exception = ExceptionDispatchInfo.Capture(ex);
			}
		})
		{
			IsBackground = true,
		};
		scheduler._thread.Start();
		bool completed = scheduler._thread.Join(TimeSpan.FromSeconds(10));
		exception?.Throw();
		return completed;
	}
}
