using System.Threading.Tasks;

namespace aweXpect.Core;

/// <summary>
///     Compares the strings of one evaluation, created by
///     <see cref="StringMatchType.ForEvaluation(MatchEnvironment, StringMatchSettings)" />.
/// </summary>
/// <remarks>
///     The items of a collection are compared one after the other with the matcher of the evaluation, so it may keep
///     state across them, e.g. which options matched nothing in any comparison.
/// </remarks>
public abstract class StringMatcher
{
	/// <summary>
	///     Compares the already normalized <paramref name="actual" /> value with the <paramref name="expected" /> value.
	/// </summary>
	/// <remarks>
	///     Return <see cref="MatchResult.NotComparable(string, System.Exception)" /> when the <paramref name="actual" />
	///     value cannot be compared at all, e.g. a string that is no valid JSON, so that the expectation fails in both
	///     polarities, instead of the negated expectation succeeding.
	/// </remarks>
	public abstract ValueTask<MatchResult> Match(string? actual, string? expected);

	/// <summary>
	///     The failure text for a comparison that returned the <paramref name="result" />, starting with
	///     <paramref name="it" />.
	/// </summary>
	/// <param name="it">The name of the subject in the result text.</param>
	/// <param name="grammars">The grammars of the expectation.</param>
	/// <param name="actual">The normalized actual value of the comparison.</param>
	/// <param name="expected">The normalized expected value of the comparison.</param>
	/// <param name="result">The result of the comparison.</param>
	/// <param name="differenceSettings">
	///     Maps a position in the normalized <paramref name="actual" /> value to the original value, when an option
	///     removed some of its whitespace.
	/// </param>
	/// <remarks>
	///     The default writes the <see cref="MatchResult.Explanation" /> of the <paramref name="result" />, or else the
	///     <paramref name="actual" /> value. A matcher that can explain a mismatch from the values alone overrides this
	///     instead of allocating an explanation for each mismatch.
	/// </remarks>
	public virtual string GetFailure(string it, ExpectationGrammars grammars, string? actual, string? expected,
		MatchResult result, StringDifferenceSettings? differenceSettings)
		=> result.Explanation?.GetFailure(it, grammars, null) ?? $"{it} was {Formatter.Format(actual)}";

	/// <summary>
	///     Adds the contexts that explain a failure of this evaluation, e.g. the options of the match type.
	/// </summary>
	/// <remarks>
	///     Only called while a failure message is created.
	/// </remarks>
	public virtual void AppendContexts(ResultContextCollector contexts)
	{
		// Most matchers have nothing to add.
	}
}
