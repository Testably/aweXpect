namespace aweXpect.Core;

/// <summary>
///     Explains why a comparison did not match, see <see cref="MatchResult.NoMatchBecause(MatchExplanation)" />.
/// </summary>
/// <remarks>
///     Created only for a mismatch, and asked for its text only when a failure message is created, so that it can keep
///     what the comparison found, e.g. the differences of two documents, without the matcher keeping state per
///     comparison.
/// </remarks>
public abstract class MatchExplanation
{
	/// <summary>
	///     The failure text, which starts with <paramref name="it" />, e.g. <c>it differed as …</c>.
	/// </summary>
	/// <param name="it">The name of the subject in the result text.</param>
	/// <param name="grammars">The grammars of the expectation.</param>
	/// <param name="indentation">
	///     The indentation for the lines after the first, when the failure text is part of a nested result, e.g. of an
	///     expectation in <c>Expect.ThatAll</c>.
	/// </param>
	public abstract string GetFailure(string it, ExpectationGrammars grammars, string? indentation);
}
