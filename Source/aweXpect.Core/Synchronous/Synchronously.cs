using System.Threading.Tasks;
using aweXpect.Results;
using static aweXpect.Core.Extending.SynchronousBoundary;

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
}
