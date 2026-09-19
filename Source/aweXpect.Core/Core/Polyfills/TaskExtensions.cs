#if NETSTANDARD2_0
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace aweXpect.Core.Polyfills;

/// <summary>
///     Provides extension methods to simplify writing platform independent tests.
/// </summary>
[ExcludeFromCodeCoverage]
internal static class TaskExtensionMethods
{
	/// <summary>
	///     Gets a <see cref="Task{TResult}" /> that will complete when the <paramref name="task" /> completes or when the
	///     <paramref name="cancellationToken" /> has cancellation requested.
	/// </summary>
	internal static async Task<TResult> WaitAsync<TResult>(
		this Task<TResult> task,
		CancellationToken cancellationToken)
	{
		if (task.IsCompleted || !cancellationToken.CanBeCanceled)
		{
			return await task;
		}

		TaskCompletionSource<bool> cancellation = new(TaskCreationOptions.RunContinuationsAsynchronously);
		using (cancellationToken.Register(() => cancellation.TrySetResult(true)))
		{
			if (await Task.WhenAny(task, cancellation.Task) != task)
			{
				cancellationToken.ThrowIfCancellationRequested();
			}
		}

		return await task;
	}
}
#endif
