namespace aweXpect.Core;

/// <summary>
///     The result of a comparison with an <see cref="IObjectMatchType" /> that explains a failure.
/// </summary>
/// <remarks>
///     A match type may return itself as the result, so that a comparison allocates no result. Such a result is only
///     valid until the next call to
///     <see cref="IObjectMatchType.AreConsideredEqualWithExplanation{TActual, TExpected}(TActual, TExpected)" /> of
///     the match type.
/// </remarks>
public interface IObjectMatchResult
{
	/// <summary>
	///     Whether the compared objects are considered equal.
	/// </summary>
	bool IsMatch { get; }

	/// <summary>
	///     Get an extended failure text for the compared <paramref name="actual" /> and <paramref name="expected" />
	///     objects.
	/// </summary>
	/// <param name="it">The name of the subject in the result text.</param>
	/// <param name="grammars">The grammars of the expectation.</param>
	/// <param name="actual">The actual object of the comparison.</param>
	/// <param name="expected">The expected object of the comparison.</param>
	/// <param name="indentation">
	///     The indentation for the lines after the first, when the failure text is part of a nested result, e.g. of an
	///     expectation in <c>Expect.ThatAll</c>. Pass on the <c>indentation</c> of <c>AppendResult</c>.
	/// </param>
	string GetExtendedFailure(string it, ExpectationGrammars grammars, object? actual, object? expected,
		string? indentation = null);
}
