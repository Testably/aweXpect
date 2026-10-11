using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace aweXpect.Core.Helpers;

internal static class TaskHelpers
{
	/// <summary>
	///     Awaits the <paramref name="task" />, unless the <paramref name="cancellationToken" /> is canceled first: then
	///     the <paramref name="task" /> is abandoned and a <see cref="TaskCanceledException" /> is thrown, as if it had
	///     been canceled.
	/// </summary>
	/// <remarks>
	///     A user-supplied task may ignore the cancellation or never complete, and awaiting it regardless would let the
	///     evaluation exceed its timeout or hang forever. A completed task or a token that cannot be canceled returns
	///     the <paramref name="task" /> itself, so that this costs nothing when no timeout or cancellation applies.
	/// </remarks>
	public static Task AbandonOnCancellation(this Task task, CancellationToken cancellationToken)
		=> task.IsCompleted || !cancellationToken.CanBeCanceled
			? task
			: AwaitOrAbandon(task, cancellationToken);

	/// <inheritdoc cref="AbandonOnCancellation(Task, CancellationToken)" />
	public static Task<TResult> AbandonOnCancellation<TResult>(this Task<TResult> task,
		CancellationToken cancellationToken)
		=> task.IsCompleted || !cancellationToken.CanBeCanceled
			? task
			: AwaitOrAbandon(task, cancellationToken);

	/// <summary>
	///     Returns the exceptions of the faulted <paramref name="task" /> except the <paramref name="exception" /> that
	///     awaiting it threw, or <see langword="null" /> when there are none.
	/// </summary>
	/// <remarks>
	///     Awaiting a task only throws its first exception, so that the others, e.g. of
	///     <see cref="Task.WhenAll(Task[])" />, would otherwise get lost.
	/// </remarks>
	public static Exception[]? GetOtherExceptions(this Task task, Exception exception)
		=> task.Exception is { InnerExceptions: { Count: > 1, } exceptions, } &&
		   ReferenceEquals(exceptions[0], exception)
			? exceptions.Skip(1).ToArray()
			: null;

	/// <summary>
	///     Waits until the <paramref name="task" /> completed, without throwing its exception, unless the
	///     <paramref name="cancellationToken" /> is canceled first: then the <paramref name="task" /> is abandoned and a
	///     <see cref="TaskCanceledException" /> is thrown.
	/// </summary>
	/// <remarks>
	///     Awaiting a faulted task rethrows its exception, which costs about as much as the original throw, so that a
	///     caller who only inspects the exception reads it from the completed <paramref name="task" /> instead.
	/// </remarks>
	public static Task WaitWithoutThrowing(this Task task, CancellationToken cancellationToken)
		=> task.IsCompleted
			? Task.CompletedTask
			: WaitOrAbandon(task, cancellationToken);

	/// <summary>
	///     Returns the first exception of the faulted <paramref name="task" /> and the
	///     <paramref name="otherExceptions" />, without rethrowing them.
	/// </summary>
	public static Exception GetFirstException(this Task task, out Exception[]? otherExceptions)
	{
		ReadOnlyCollection<Exception> exceptions = task.Exception!.InnerExceptions;
		otherExceptions = exceptions.Count > 1 ? exceptions.Skip(1).ToArray() : null;
		return exceptions[0];
	}

	private static async Task<TResult> AwaitOrAbandon<TResult>(Task<TResult> task,
		CancellationToken cancellationToken)
	{
		await WaitOrAbandon(task, cancellationToken);
		return await task;
	}

	private static async Task AwaitOrAbandon(Task task, CancellationToken cancellationToken)
	{
		await WaitOrAbandon(task, cancellationToken);
		await task;
	}

	/// <remarks>
	///     The outcome of the <paramref name="task" /> wins when it completed by the time the cancellation is noticed,
	///     so that a task which reacts to the cancellation itself keeps reporting its own exception.
	/// </remarks>
	private static async Task WaitOrAbandon(Task task, CancellationToken cancellationToken)
	{
#if NET8_0_OR_GREATER
		await task.WaitAsync(cancellationToken)
			.ConfigureAwait(ConfigureAwaitOptions.ContinueOnCapturedContext | ConfigureAwaitOptions.SuppressThrowing);
#else
		TaskCompletionSource<bool> cancellation = new(TaskCreationOptions.RunContinuationsAsynchronously);
		using (cancellationToken.Register(static state => ((TaskCompletionSource<bool>)state!).TrySetResult(true),
			       cancellation))
		{
			await Task.WhenAny(task, cancellation.Task);
		}
#endif

		if (!task.IsCompleted)
		{
			throw Abandon(task);
		}
	}

	/// <summary>
	///     Abandons the running <paramref name="task" /> and returns the exception to throw instead of its outcome.
	/// </summary>
	/// <remarks>
	///     An abandoned task keeps running, so its exception is observed, as nobody else awaits it and it would
	///     otherwise surface as <see cref="TaskScheduler.UnobservedTaskException" />.
	/// </remarks>
	private static TaskCanceledException Abandon(Task task)
	{
		_ = task.ContinueWith(static t => _ = t.Exception, CancellationToken.None,
			TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously,
			TaskScheduler.Default);
		return new TaskCanceledException();
	}
}
