using aweXpect.Core;

namespace aweXpect.Helpers;

/// <summary>
///     Options that wrap the evaluator of an evaluation, e.g. to let the comparer of a set subject decide.
/// </summary>
internal interface IWrapsEqualityEvaluator<T>
{
	/// <summary>
	///     Returns the evaluator to compare with, which wraps the <paramref name="evaluator" /> when it has to.
	/// </summary>
	IEqualityEvaluator<T> Wrap(IEqualityEvaluator<T> evaluator);
}
