using System;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core.Helpers;

namespace aweXpect.Signaling;

/// <summary>
///     Waits for the signals of a waiter, which completes its task when enough signals were received.
/// </summary>
/// <remarks>
///     A wait ends at the <c>timeout</c> or at the cancellation without throwing, so that the caller returns the
///     signals received until then.
/// </remarks>
internal static class SignalWait
{
	/// <summary>
	///     Creates the task source of a waiter, whose continuations never run on the thread that signals.
	/// </summary>
	public static TaskCompletionSource<bool> CreateCompletion()
		=> new(TaskCreationOptions.RunContinuationsAsynchronously);

	/// <summary>
	///     Blocks the current thread until the <paramref name="completion" /> completes, the <paramref name="timeout" />
	///     expires or the <paramref name="cancellationToken" /> is canceled.
	/// </summary>
	public static void Block(Task completion, TimeSpan timeout, CancellationToken cancellationToken)
	{
		try
		{
			completion.Wait(ToMilliseconds(timeout), cancellationToken);
		}
		catch (OperationCanceledException)
		{
			// The signals received until the cancellation are returned.
		}
	}

	/// <summary>
	///     Waits without blocking a thread until the <paramref name="completion" /> completes, the
	///     <paramref name="timeout" /> expires or the <paramref name="cancellationToken" /> is canceled.
	/// </summary>
	public static async Task WaitAsync(Task completion, TimeSpan timeout, CancellationToken cancellationToken)
	{
		if (completion.IsCompleted)
		{
			return;
		}

		using CancellationTokenSource delayCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
		Task delay = Task.Delay(timeout.ToTimerTimeout(), delayCts.Token);
		await Task.WhenAny(completion, delay);
		// Releases the timer of the delay, which would otherwise run until the timeout.
		delayCts.Cancel();
	}

	private static int ToMilliseconds(TimeSpan timeout)
	{
		TimeSpan timerTimeout = timeout.ToTimerTimeout();
		return timerTimeout == Timeout.InfiniteTimeSpan ? Timeout.Infinite : (int)timerTimeout.TotalMilliseconds;
	}
}
