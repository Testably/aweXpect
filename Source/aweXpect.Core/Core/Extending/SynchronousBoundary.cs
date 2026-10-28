using System;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core.Helpers;

namespace aweXpect.Core.Extending;

/// <summary>
///     Waits for asynchronous code where an extension has to answer synchronously, e.g. in an
///     <see cref="System.Collections.Generic.IEnumerable{T}" /> or in a synchronous callback.
/// </summary>
public static class SynchronousBoundary
{
	/// <summary>
	///     Starts the <paramref name="operation" /> and waits for its result.
	/// </summary>
	/// <remarks>
	///     When the caller has a <see cref="SynchronizationContext" /> or a <see cref="TaskScheduler" />, e.g. the UI
	///     thread of WPF or WinForms, the <paramref name="operation" /> starts without them, so that its continuations do
	///     not wait for the blocked caller.
	/// </remarks>
	public static TResult Wait<TResult>(Func<ValueTask<TResult>> operation)
	{
		operation.ThrowIfNull();
		ValueTask<TResult> result = CallerSchedulesContinuations() ? StartDetached(operation) : operation();
		return result.IsCompletedSuccessfully ? result.Result : WaitForCompletion(result);
	}

	private static TResult WaitForCompletion<TResult>(ValueTask<TResult> pending)
		=> pending.AsTask().GetAwaiter().GetResult();

	internal static bool CallerSchedulesContinuations()
		=> SynchronizationContext.Current is not null || TaskScheduler.Current != TaskScheduler.Default;

	/// <summary>
	///     Calls the <paramref name="start" /> on the current thread, but without the <see cref="SynchronizationContext" /> and the
	///     <see cref="TaskScheduler" /> of the caller, because a continuation scheduled to the blocked caller would never
	///     run.
	/// </summary>
	internal static TAwaiter StartDetached<TAwaiter>(Func<TAwaiter> start)
	{
		SynchronizationContext? context = SynchronizationContext.Current;
		SynchronizationContext.SetSynchronizationContext(null);
		try
		{
			Task<TAwaiter> task = new(start);
			task.RunSynchronously(TaskScheduler.Default);
			return task.GetAwaiter().GetResult();
		}
		finally
		{
			SynchronizationContext.SetSynchronizationContext(context);
		}
	}
}
