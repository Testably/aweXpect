namespace aweXpect.Core;

/// <summary>
///     Defines how two strings are compared: configured once, and asked for a <see cref="StringMatcher" /> for each
///     evaluation.
/// </summary>
/// <remarks>
///     Set it with <c>SetMatchType</c> on the <see cref="aweXpect.Options.StringEqualityOptions" />. The match type
///     itself keeps no state of a comparison, as one instance serves every evaluation of the expectation; state of an
///     evaluation belongs into its matcher, and state of a single comparison into the
///     <see cref="MatchExplanation" /> of its <see cref="MatchResult" />.
/// </remarks>
public abstract class StringMatchType
{
	/// <summary>
	///     The edges of the subject that the expected value lines up with.
	/// </summary>
	/// <remarks>
	///     The whitespace options remove the leading or trailing whitespace of the expected value only at an anchored
	///     edge. The default is <see cref="StringMatchAnchors.StartAndEnd" />, as for an expected value that covers the
	///     whole subject.
	/// </remarks>
	public virtual StringMatchAnchors Anchors => StringMatchAnchors.StartAndEnd;

	/// <summary>
	///     Indicates whether the match type inspects the content of the subject instead of comparing it as a value.
	/// </summary>
	/// <remarks>
	///     A <see langword="null" /> subject has no content to inspect, so an inspecting match type fails for it in both
	///     polarities, exactly like <c>StartsWith</c> and <c>DoesNotStartWith</c> do. A match type that compares the
	///     subject as a value, e.g. a custom equality, returns <see langword="false" />, so that a <see langword="null" />
	///     subject is equal to a <see langword="null" /> expected value and different from every other value.
	/// </remarks>
	public virtual bool InspectsSubject => false;

	/// <summary>
	///     Indicates whether a failure lists the lines that differ, when the values have several lines.
	/// </summary>
	/// <remarks>
	///     The lines are compared with the casing and the whitespace options, so only a match type that compares the
	///     values for equality returns <see langword="true" />. The default is <see langword="false" />.
	/// </remarks>
	public virtual bool ListsLineDifferences => false;

	/// <summary>
	///     The words with which the match type describes its comparison.
	/// </summary>
	/// <remarks>
	///     Core writes the expectation text for every grammar from them, followed by the casing and the comparer that
	///     are specified, e.g. <c>starts with "Let it" ignoring case</c>.
	/// </remarks>
	public abstract MatchPhrase Phrase { get; }

	/// <summary>
	///     Indicates whether a failure hints that the values would match when the newline style is ignored.
	/// </summary>
	/// <remarks>
	///     The hint is given for a match type whose failure points at the first difference, which would be the first
	///     <c>\r</c>. The default is <see langword="false" />.
	/// </remarks>
	public virtual bool SuggestsIgnoringNewlineStyle => false;

	/// <summary>
	///     The settings that the match type honours.
	/// </summary>
	/// <remarks>
	///     Core rejects every other enabled setting with the same message, e.g.
	///     <c>IgnoringCase cannot be combined with AsJson.</c>, at the call that specifies the conflict, whether the match
	///     type or the setting is specified first; a match type that follows several such settings names all of them,
	///     e.g. <c>IgnoringCase and IgnoringIndentation cannot be combined with AsJson.</c> The default is
	///     <see cref="StringMatchSettingKinds.WhiteSpace" />, as Core applies the whitespace options to both values before
	///     the matcher sees them.
	/// </remarks>
	public virtual StringMatchSettingKinds SupportedSettings => StringMatchSettingKinds.WhiteSpace;

	/// <summary>
	///     Creates the matcher that compares the strings of one evaluation in the <paramref name="environment" /> with
	///     the <paramref name="settings" />.
	/// </summary>
	/// <remarks>
	///     A match type without state per evaluation returns a shared matcher, so that an evaluation allocates nothing.
	///     The <paramref name="settings" /> only hold what the match type supports.
	/// </remarks>
	public abstract StringMatcher ForEvaluation(MatchEnvironment environment, StringMatchSettings settings);

	/// <summary>
	///     Rejects an <paramref name="expected" /> value that the match type cannot use, by throwing, e.g. an
	///     <see cref="System.ArgumentException" /> for the <paramref name="parameterName" /> of the caller.
	/// </summary>
	/// <remarks>
	///     This receives the value as <see cref="StringMatcher.Match(string?, string?)" /> does, and is called before a
	///     subject is compared with it and also when there is none to compare, e.g. for an empty collection, so that an
	///     unusable value is rejected whichever subject it is verified for. Core already rejected a
	///     <see langword="null" /> pattern, see <see cref="MatchPhrase.WithPattern(string, string)" />.<br />
	///     It is called again for every item of a collection that is compared with the value, so it should be cheap
	///     for a value that it already accepted.
	/// </remarks>
	public virtual void ValidateExpected(string? expected, string parameterName)
	{
		// Every value is usable unless a match type says otherwise.
	}
}
