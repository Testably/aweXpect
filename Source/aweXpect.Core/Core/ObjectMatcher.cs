using System.Threading.Tasks;

namespace aweXpect.Core;

/// <summary>
///     Compares the objects of one evaluation, created by <see cref="ObjectMatchType.ForEvaluation(MatchEnvironment)" />.
/// </summary>
/// <remarks>
///     The items of a collection are compared one after the other with the matcher of the evaluation, so it may keep
///     state across them, e.g. which options matched nothing in any comparison.
/// </remarks>
public abstract class ObjectMatcher
{
	/// <summary>
	///     Decides whether the <paramref name="actual" /> value matches the <paramref name="expected" /> value.
	/// </summary>
	/// <remarks>
	///     Most comparisons, e.g. of the items of a collection, are never explained, so this only needs to decide.
	///     Return <see cref="MatchResult.NotComparable(string, System.Exception)" /> when the
	///     <paramref name="actual" /> value cannot be compared at all, so that the expectation fails in both polarities.
	/// </remarks>
	public abstract ValueTask<MatchResult> Match<TActual, TExpected>(TActual actual, TExpected expected);

	/// <summary>
	///     Compares the <paramref name="actual" /> value with the <paramref name="expected" /> value like
	///     <see cref="Match{TActual, TExpected}(TActual, TExpected)" />, for a comparison whose failure is explained.
	/// </summary>
	/// <remarks>
	///     Override it when explaining a mismatch costs more than deciding it, e.g. to collect all differences of an
	///     equivalency comparison into a <see cref="MatchExplanation" />.
	/// </remarks>
	public virtual ValueTask<MatchResult> MatchWithExplanation<TActual, TExpected>(TActual actual, TExpected expected)
		=> Match(actual, expected);

	/// <summary>
	///     The failure text for a comparison that returned the <paramref name="result" />, starting with
	///     <paramref name="it" />.
	/// </summary>
	/// <param name="it">The name of the subject in the result text.</param>
	/// <param name="grammars">The grammars of the expectation.</param>
	/// <param name="actual">The actual object of the comparison.</param>
	/// <param name="expected">The expected object of the comparison.</param>
	/// <param name="result">The result of the comparison.</param>
	/// <param name="indentation">
	///     The indentation for the lines after the first, when the failure text is part of a nested result, e.g. of an
	///     expectation in <c>Expect.ThatAll</c>.
	/// </param>
	/// <remarks>
	///     The default writes the <see cref="MatchResult.Explanation" /> of the <paramref name="result" />, or else the
	///     <paramref name="actual" /> value.
	/// </remarks>
	public virtual string GetFailure(string it, ExpectationGrammars grammars, object? actual, object? expected,
		MatchResult result, string? indentation)
		=> result.Explanation?.GetFailure(it, grammars, indentation) ??
		   $"{it}{grammars.SubjectVerb(it, " was ", " were ")}{Formatter.Format(actual, FormattingOptions.Indented(indentation))}";

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
