using System.Collections.Concurrent;
using System.Threading;

namespace aweXpect.TestHelpers;

/// <remarks>
///     Runs all continuations on one dedicated thread, like the UI thread of WPF or WinForms does.
/// </remarks>
internal sealed class SingleThreadedSynchronizationContext : SynchronizationContext
{
	private readonly BlockingCollection<(SendOrPostCallback Callback, object? State)> _queue = new();

	public override void Post(SendOrPostCallback d, object? state) => _queue.Add((d, state));

	public static Task Run(Func<Task> action)
		=> Run(async () =>
		{
			await action();
			return true;
		});

	/// <remarks>
	///     It gives up after ten seconds, so that a deadlock fails the test instead of hanging the test run.
	/// </remarks>
	public static async Task<T> Run<T>(Func<Task<T>> action)
	{
		TaskCompletionSource<Task<T>> started = new(TaskCreationOptions.RunContinuationsAsynchronously);
		Thread thread = new(() =>
		{
			SingleThreadedSynchronizationContext context = new();
			SetSynchronizationContext(context);
			Task<T> task = action();
			task.ContinueWith(_ => context._queue.CompleteAdding(), TaskScheduler.Default);
			foreach ((SendOrPostCallback callback, object? state) in context._queue.GetConsumingEnumerable())
			{
				callback(state);
			}

			started.SetResult(task);
		})
		{
			IsBackground = true,
		};
		thread.Start();
		if (await Task.WhenAny(started.Task, Task.Delay(TimeSpan.FromSeconds(10))) != started.Task)
		{
			throw new TimeoutException("The action did not complete on the synchronization context within ten seconds.");
		}

		return await await started.Task;
	}
}
