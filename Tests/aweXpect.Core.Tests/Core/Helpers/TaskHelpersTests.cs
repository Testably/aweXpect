using System.Collections.Concurrent;
using System.Threading;
using aweXpect.Core.Helpers;

namespace aweXpect.Core.Tests.Core.Helpers;

public sealed class TaskHelpersTests
{
	[Test]
	public async Task AbandonOnCancellation_WhenTheTaskReactsToTheCancellationWithItsOwnException_ShouldReportIt()
	{
		TaskCompletionSource<bool> source = new();
		using CancellationTokenSource cts = new();
		using CancellationTokenRegistration _ = cts.Token.Register(()
			=> source.SetException(new InvalidOperationException("reacted to the cancellation")));
		using ManualSynchronizationContext context = new();
		Task wait = context.Start(() => ((Task)source.Task).AbandonOnCancellation(cts.Token));

		cts.Cancel();
		context.RunUntilCompleted(wait);

		await That(() => wait).ThrowsExactly<InvalidOperationException>()
			.WithMessage("reacted to the cancellation")
			.Because("a task that fails at the cancellation reacted to it, so its own exception is reported");
	}

	[Test]
	public async Task WaitWithoutThrowing_WhenTheTaskCompletesBeforeTheCancellationIsNoticed_ShouldAbandonIt()
	{
		TaskCompletionSource<bool> source = new();
		using CancellationTokenSource cts = new();
		using CancellationTokenRegistration _ = cts.Token.Register(() => source.SetResult(true));
		using ManualSynchronizationContext context = new();
		Task wait = context.Start(() => source.Task.WaitWithoutThrowing(cts.Token));

		cts.Cancel();
		context.RunUntilCompleted(wait);

		await That(() => wait).ThrowsExactly<TaskCanceledException>()
			.WithMessage(new TaskCanceledException().Message)
			.Because("the task was still running at the cancellation, like a task that a virtual clock completes on the way");
	}

	/// <summary>
	///     A synchronization context that only runs its continuations in <see cref="RunUntilCompleted(Task)" />, so that
	///     the cancellation is noticed after <see cref="CancellationTokenSource.Cancel()" /> ran all callbacks, of which
	///     the earlier registered ones complete the task.
	/// </summary>
	private sealed class ManualSynchronizationContext : SynchronizationContext, IDisposable
	{
		private readonly BlockingCollection<(SendOrPostCallback Callback, object? State)> _posted = new();

		public void Dispose() => _posted.Dispose();

		public override void Post(SendOrPostCallback d, object? state) => _posted.Add((d, state));

		public void RunUntilCompleted(Task task)
		{
			SynchronizationContext? previous = Current;
			SetSynchronizationContext(this);
			try
			{
				for (int attempt = 0; attempt < 1000 && !task.IsCompleted; attempt++)
				{
					if (_posted.TryTake(out (SendOrPostCallback Callback, object? State) posted, 10))
					{
						posted.Callback(posted.State);
					}
				}
			}
			finally
			{
				SetSynchronizationContext(previous);
			}
		}

		public Task Start(Func<Task> startWait)
		{
			SynchronizationContext? previous = Current;
			SetSynchronizationContext(this);
			try
			{
				return startWait();
			}
			finally
			{
				SetSynchronizationContext(previous);
			}
		}
	}
}
