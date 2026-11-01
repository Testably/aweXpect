using System.Threading;
using aweXpect.Core.Helpers;

namespace aweXpect.Core.Tests.Core.Helpers;

public sealed class TaskHelpersTests
{
	[Test]
	public async Task WaitWithoutThrowing_WhenTheTaskCompletesBeforeTheCancellationIsNoticed_ShouldAbandonIt()
	{
		TaskCompletionSource<bool> source = new();
		using CancellationTokenSource cts = new();
		Task wait = WaitOnPostingContext(source.Task, cts.Token);

		cts.Cancel();
		source.SetResult(true);

		await That(() => wait).ThrowsExactly<TaskCanceledException>()
			.WithMessage(new TaskCanceledException().Message)
			.Because("the task was still running at the cancellation, even if it completed before it was noticed");
	}

	/// <summary>
	///     Starts the wait on a synchronization context that posts its continuations to the thread pool, so that the
	///     cancellation is noticed after the test continued.
	/// </summary>
	private static Task WaitOnPostingContext(Task task, CancellationToken cancellationToken)
	{
		SynchronizationContext? previous = SynchronizationContext.Current;
		SynchronizationContext.SetSynchronizationContext(new PostingSynchronizationContext());
		try
		{
			return task.WaitWithoutThrowing(cancellationToken);
		}
		finally
		{
			SynchronizationContext.SetSynchronizationContext(previous);
		}
	}

	private sealed class PostingSynchronizationContext : SynchronizationContext;
}
