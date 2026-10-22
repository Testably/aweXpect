using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Results;
using static aweXpect.Core.Extending.SynchronousBoundary;

namespace aweXpect.Synchronous;

/// <summary>
///     Methods to support synchronous execution.
/// </summary>
/// <remarks>
///     <b>WARNING!</b><br />
///     It blocks the thread until the expectation is evaluated, so only use it where awaiting is impossible, e.g.
///     for a <c>ref struct</c> such as <c>Span&lt;T&gt;</c>, inside a <c>lock</c> or in a synchronous callback.
/// </remarks>
public static class Synchronously
{
	/// <summary>
	///     Verifies synchronously that the expectation is satisfied.
	/// </summary>
	/// <remarks>
	///     <b>WARNING!</b><br />
	///     It blocks the thread until the expectation is evaluated, so only use it where awaiting is impossible, e.g.
///     for a <c>ref struct</c> such as <c>Span&lt;T&gt;</c>, inside a <c>lock</c> or in a synchronous callback.
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
	///     It blocks the thread until the expectation is evaluated, so only use it where awaiting is impossible, e.g.
///     for a <c>ref struct</c> such as <c>Span&lt;T&gt;</c>, inside a <c>lock</c> or in a synchronous callback.
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

	/// <summary>
	///     Verifies synchronously that the <paramref name="combination" /> of expectations is satisfied.
	/// </summary>
	/// <remarks>
	///     <b>WARNING!</b><br />
	///     It blocks the thread until the expectation is evaluated, so only use it where awaiting is impossible, e.g.
///     for a <c>ref struct</c> such as <c>Span&lt;T&gt;</c>, inside a <c>lock</c> or in a synchronous callback.
	/// </remarks>
	public static void Verify(Expectation.Combination combination)
	{
		if (CallerSchedulesContinuations())
		{
			StartDetached(combination.GetAwaiter).GetResult();
			return;
		}

		combination.GetAwaiter().GetResult();
	}
}
