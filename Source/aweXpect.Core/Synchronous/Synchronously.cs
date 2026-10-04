using System;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Results;

namespace aweXpect.Synchronous;

/// <summary>
///     Methods to support synchronous execution.
/// </summary>
/// <remarks>
///     <b>WARNING!</b><br />
///     The only intended use case is to support synchronous evaluation for <c>ref struct</c>.
/// </remarks>
public static class Synchronously
{
	/// <summary>
	///     Verifies synchronously that the expectation is satisfied.
	/// </summary>
	/// <remarks>
	///     <b>WARNING!</b><br />
	///     The only intended use case is to support synchronous evaluation for <c>ref struct</c>.
	/// </remarks>
	public static void Verify(ExpectationResult result)
	{
		if (CallerSchedulesContinuations())
		{
			StartDetached(result.GetAwaiter).GetResult();
			return;
		}

		result.GetAwaiter().GetResult();
	}

	/// <summary>
	///     Verifies synchronously that the expectation is satisfied.
	/// </summary>
	/// <remarks>
	///     <b>WARNING!</b><br />
	///     The only intended use case is to support synchronous evaluation for <c>ref struct</c>.
	/// </remarks>
	public static TType Verify<TType, TSelf>(ExpectationResult<TType, TSelf> result)
		where TSelf : ExpectationResult<TType, TSelf>
	{
		if (CallerSchedulesContinuations())
		{
			(bool isDetachedMet, TType detachedValue, Task<TType>? detachedEvaluation) = StartDetached(()
				=> (result.TryGetValueRightAway(out TType value, out Task<TType>? evaluation), value, evaluation));
			return isDetachedMet ? detachedValue : detachedEvaluation!.GetAwaiter().GetResult();
		}

		return result.TryGetValueRightAway(out TType metValue, out Task<TType>? pendingEvaluation)
			? metValue
			: pendingEvaluation!.GetAwaiter().GetResult();
	}

	private static bool CallerSchedulesContinuations()
		=> SynchronizationContext.Current is not null || TaskScheduler.Current != TaskScheduler.Default;

	/// <summary>
	///     Starts the evaluation on the current thread, but without the <see cref="SynchronizationContext" /> and the
	///     <see cref="TaskScheduler" /> of the caller, because a continuation scheduled to the blocked caller would never
	///     run.
	/// </summary>
	private static TAwaiter StartDetached<TAwaiter>(Func<TAwaiter> start)
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
