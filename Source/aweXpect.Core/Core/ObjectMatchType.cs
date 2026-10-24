namespace aweXpect.Core;

/// <summary>
///     Defines how two objects are compared: configured once, and asked for an <see cref="ObjectMatcher" /> for each
///     evaluation.
/// </summary>
/// <remarks>
///     Set it with <c>SetMatchType</c> on the <see cref="aweXpect.Options.ObjectEqualityOptions{TSubject}" />. The
///     match type itself keeps no state of a comparison, as one instance serves every evaluation of the expectation;
///     state of an evaluation belongs into its matcher, and state of a single comparison into the
///     <see cref="MatchExplanation" /> of its <see cref="MatchResult" />.
/// </remarks>
public abstract class ObjectMatchType
{
	/// <summary>
	///     Creates the matcher that compares the objects of one evaluation in the <paramref name="environment" />.
	/// </summary>
	/// <remarks>
	///     A match type without state per evaluation returns a shared matcher, so that an evaluation allocates nothing.
	/// </remarks>
	public abstract ObjectMatcher ForEvaluation(MatchEnvironment environment);

	/// <summary>
	///     Get the expectations text.
	/// </summary>
	public abstract string GetExpectation(string expected, ExpectationGrammars grammars);

	/// <summary>
	///     Get the expectation text for a single expected item, e.g. <c>equivalent to {expected}</c>.
	/// </summary>
	/// <remarks>
	///     A match type that describes the item instead of only formatting it prepends the
	///     <paramref name="itemNoun" />, so that a verb such as <c>contains</c> keeps a direct object.<br />
	///     A match type that only formats the value prepends the <paramref name="comparison" /> instead, so that a verb
	///     which already names the item reads <c>has item equal to 3</c>.
	/// </remarks>
	public abstract string PrependItemAndComparison(string expected, string? itemNoun = null,
		string? comparison = null);
}
