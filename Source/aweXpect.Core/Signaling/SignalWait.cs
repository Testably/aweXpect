using System;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core.Helpers;

namespace aweXpect.Signaling;

/// <summary>
///     The wait of a waiter, which completes it when enough signals were received.
/// </summary>
/// <remarks>
///     A wait ends at the <c>timeout</c> or at the cancellation without throwing, so that the caller returns the
///     signals received until then.
/// </remarks>
internal sealed class SignalWait : IDisposable
{
	/// <summary>
	///     Wakes a blocked thread on the thread that signals.
	/// </summary>
	/// <remarks>
	///     The <see cref="_completion" /> cannot do that: on .NET Framework a thread that blocks on its task is woken
	///     through the thread pool, so that the wait would last until one of its threads is free.
	/// </remarks>
	private readonly ManualResetEventSlim _completed = new();

	/// <summary>
	///     Completes an asynchronous wait, whose continuations never run on the thread that signals.
	/// </summary>
	private readonly TaskCompletionSource<bool> _completion = new(TaskCreationOptions.RunContinuationsAsynchronously);

	/// <inheritdoc cref="IDisposable.Dispose()" />
	public void Dispose() => _completed.Dispose();

	/// <summary>
	///     Ends the wait, because enough signals were received.
	/// </summary>
	public void Complete()
	{
		_completed.Set();
		_completion.TrySetResult(true);
	}

	/// <summary>
	///     Blocks the current thread until the wait is completed, the <paramref name="timeout" /> expires or the
	///     <paramref name="cancellationToken" /> is canceled.
	/// </summary>
	public void Block(TimeSpan timeout, CancellationToken cancellationToken)
	{
		try
		{
			_completed.Wait(ToMilliseconds(timeout), cancellationToken);
		}
		catch (OperationCanceledException)
		{
			// The signals received until the cancellation are returned.
		}
	}

	/// <summary>
	///     Waits without blocking a thread until the wait is completed, the <paramref name="timeout" /> expires or the
	///     <paramref name="cancellationToken" /> is canceled.
	/// </summary>
	public async Task WaitAsync(TimeSpan timeout, CancellationToken cancellationToken)
	{
		Task completion = _completion.Task;
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
