namespace aweXpect.Core;

/// <summary>
///     Defines how a <typeparamref name="TSubject" /> is compared: configured once, and asked for an
///     <see cref="ObjectMatcher{TSubject}" /> for each evaluation.
/// </summary>
/// <remarks>
///     Set it with <c>SetMatchType</c> on the <see cref="aweXpect.Options.ObjectEqualityOptions{TSubject}" />. The
///     match type itself keeps no state of a comparison, as one instance serves every evaluation of the expectation;
///     state of an evaluation belongs into its matcher, and state of a single comparison into the
///     <see cref="MatchExplanation" /> of its <see cref="MatchResult" />.
/// </remarks>
public abstract class ObjectMatchType<TSubject>
{
	/// <summary>
	///     The words with which the match type describes its comparison.
	/// </summary>
	/// <remarks>
	///     Core writes the expectation text for every grammar from them, and the text for a single expected item, e.g.
	///     <c>contains an item equivalent to …</c>.
	/// </remarks>
	public abstract MatchPhrase Phrase { get; }

	/// <summary>
	///     Creates the matcher that compares the subjects of one evaluation in the <paramref name="environment" />.
	/// </summary>
	/// <remarks>
	///     A match type without state per evaluation returns a shared matcher, so that an evaluation allocates nothing.
	/// </remarks>
	public abstract ObjectMatcher<TSubject> ForEvaluation(MatchEnvironment environment);
}
