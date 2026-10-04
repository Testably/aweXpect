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
	string GetExtendedFailure(string it, ExpectationGrammars grammars, object? actual, object? expected);
}
