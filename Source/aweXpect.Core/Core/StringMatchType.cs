using System.Collections.Generic;

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
	///     The words with which the match type describes its comparison.
	/// </summary>
	/// <remarks>
	///     Core writes the expectation text for every grammar from them, followed by the casing and the comparer that
	///     are specified, e.g. <c>starts with "Let it" ignoring case</c>.
	/// </remarks>
	public abstract MatchPhrase Phrase { get; }

	/// <summary>
	///     Creates the matcher that compares the strings of one evaluation in the <paramref name="environment" /> with
	///     the <paramref name="settings" />.
	/// </summary>
	/// <remarks>
	///     A match type without state per evaluation returns a shared matcher, so that an evaluation allocates nothing.
	/// </remarks>
	public abstract StringMatcher ForEvaluation(MatchEnvironment environment, StringMatchSettings settings);

	/// <summary>
	///     Rejects options that the match type cannot honour, by throwing.
	/// </summary>
	/// <remarks>
	///     This is called when the match type is set, with the casing and the comparer that are specified so far, and
	///     before either of them changes afterwards, so that a conflict throws at the call that specifies it, in either
	///     order. It is never called with both <paramref name="ignoreCase" /> and a <paramref name="comparer" />.
	/// </remarks>
	public virtual void ValidateOptions(bool ignoreCase, IEqualityComparer<string>? comparer)
	{
		// Every option is honoured unless a match type says otherwise.
	}

	/// <summary>
	///     Rejects an <paramref name="expected" /> value that the match type cannot use, by throwing.
	/// </summary>
	/// <remarks>
	///     This receives the value as <see cref="StringMatcher.Match(string?, string?)" /> does, and is called before a
	///     subject is compared with it and also when there is none to compare, e.g. for an empty collection, so that an
	///     unusable value is rejected whichever subject it is verified for.<br />
	///     It is called again for every item of a collection that is compared with the value, so it should be cheap
	///     for a value that it already accepted.
	/// </remarks>
	public virtual void ValidateExpected(string? expected)
	{
		// Every value is usable unless a match type says otherwise.
	}
}
