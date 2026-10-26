using System.Threading.Tasks;

namespace aweXpect.Core;

/// <summary>
///     Determines if two objects are considered equal during one evaluation.
/// </summary>
/// <remarks>
///     Returned by <see cref="IEqualityOptions{TElement}.ForEvaluation" />, e.g. the
///     <see cref="ObjectMatcher{TElement}" /> of an <see cref="ObjectMatchType{TElement}" />.
/// </remarks>
public interface IEqualityEvaluator<in TElement>
{
	/// <summary>
	///     Returns <see langword="true" /> if the two objects <paramref name="actual" /> and <paramref name="expected" /> are
	///     considered equal; otherwise <see langword="false" />.
	/// </summary>
	/// <remarks>
	///     Only decides, as the items of a collection are never explained one by one. A constraint that explains a
	///     single comparison keeps the <see cref="MatchResult" /> of <c>MatchWithExplanation</c> on the
	///     <see cref="aweXpect.Options.StringEqualityOptions" /> or of
	///     <see cref="ObjectMatcher{TElement}.CompareWithExplanation{TExpected}(TElement, TExpected)" /> instead.
	/// </remarks>
	ValueTask<bool>
		AreConsideredEqual<TExpected>(TElement actual, TExpected expected);

	/// <summary>
	///     Adds the contexts that explain a failure of a comparison with these options, e.g. the equivalency options.
	/// </summary>
	/// <remarks>
	///     Only called while a failure message is created.
	/// </remarks>
	void AppendContexts(ResultContextCollector contexts);
}
