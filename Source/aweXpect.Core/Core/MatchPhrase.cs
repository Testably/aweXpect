using aweXpect.Core.Helpers;

namespace aweXpect.Core;

/// <summary>
///     The words with which a match type describes its comparison, from which Core writes the expectation text for
///     every grammar.
/// </summary>
/// <remarks>
///     Create it once, e.g. in a <see langword="static readonly" /> field, as Core reads it for every expectation text,
///     and for a pattern also for every item of a collection.
/// </remarks>
public sealed class MatchPhrase
{
	private readonly string _active;
	private readonly string _activePlural;
	private readonly bool _isEquality;
	private readonly string _negated;
	private readonly string _negatedActive;
	private readonly string _negatedActivePlural;
	private readonly string _passive;

	private MatchPhrase(string active, string activePlural, string passive,
		string negatedActive, string negatedActivePlural, string negated, bool isEquality)
	{
		_active = active;
		_activePlural = activePlural;
		_passive = passive;
		_negatedActive = negatedActive;
		_negatedActivePlural = negatedActivePlural;
		_negated = negated;
		_isEquality = isEquality;
	}

	private MatchPhrase(MatchPhrase phrase)
		: this(phrase._active, phrase._activePlural, phrase._passive, phrase._negatedActive,
			phrase._negatedActivePlural, phrase._negated, phrase._isEquality)
	{
		Name = phrase.Name;
		Suffix = phrase.Suffix;
		PatternItemPrefix = phrase.PatternItemPrefix;
		PatternNoun = phrase.PatternNoun;
		FormatsExpected = phrase.FormatsExpected;
	}

	/// <summary>
	///     The comparison by equality, e.g. <c>is equal to 3</c>.
	/// </summary>
	/// <remarks>
	///     A text that describes a single expected item may name the equality or leave it out, e.g. <c>contains 3</c> or
	///     <c>has an item equal to 3</c>, while every other comparison is always named.
	/// </remarks>
	public static MatchPhrase EqualTo { get; } = new("is equal to", "are equal to", "equal to",
		"is not equal to", "are not equal to", "not equal to", true);

	/// <summary>
	///     Whether the expected value is formatted, i.e. quoted and shortened, or written as it is.
	/// </summary>
	internal bool FormatsExpected { get; private set; } = true;

	/// <summary>
	///     The short description of the match type, e.g. <c>" as wildcard"</c>.
	/// </summary>
	internal string Name { get; private set; } = "";

	/// <summary>
	///     The prefix of an expected item that is a pattern, e.g. <c>"wildcard "</c>, or an empty string.
	/// </summary>
	internal string PatternItemPrefix { get; private set; } = "";

	/// <summary>
	///     The noun for an expected value that is a pattern, e.g. <c>"wildcard pattern"</c>, or <see langword="null" />.
	/// </summary>
	internal string? PatternNoun { get; private set; }

	/// <summary>
	///     The text after the expected value, e.g. <c>" as block"</c>.
	/// </summary>
	internal string Suffix { get; private set; } = "";

	/// <summary>
	///     A comparison that reads <c>is <paramref name="comparison" /> …</c>, e.g. <c>"equivalent to"</c> in
	///     <c>is equivalent to …</c>, <c>are equivalent to …</c> or <c>not equivalent to …</c>.
	/// </summary>
	/// <exception cref="System.ArgumentNullException">The <paramref name="comparison" /> is <see langword="null" />.</exception>
	public static MatchPhrase Being(string comparison)
	{
		comparison.ThrowIfNull();
		return new MatchPhrase($"is {comparison}", $"are {comparison}", comparison,
			$"is not {comparison}", $"are not {comparison}", $"not {comparison}", false);
	}

	/// <summary>
	///     A comparison with a verb of its own, e.g. <c>"starts with"</c>, <c>"start with"</c> and
	///     <c>"starting with"</c>, which is negated as <c>does not start with …</c>, <c>do not start with …</c> or
	///     <c>not starting with …</c>.
	/// </summary>
	/// <param name="singular">The verb for a single subject, e.g. <c>"starts with"</c>.</param>
	/// <param name="plural">The verb for several subjects, e.g. <c>"start with"</c>.</param>
	/// <param name="participle">The verb without a subject, e.g. <c>"starting with"</c>.</param>
	/// <exception cref="System.ArgumentNullException">One of the verbs is <see langword="null" />.</exception>
	public static MatchPhrase Verb(string singular, string plural, string participle)
	{
		singular.ThrowIfNull();
		plural.ThrowIfNull();
		participle.ThrowIfNull();
		return new MatchPhrase(singular, plural, participle,
			$"does not {plural}", $"do not {plural}", $"not {participle}", false);
	}

	/// <summary>
	///     Names the match type in a short description of the options, e.g. <c>" as wildcard"</c> in
	///     <c>"foo" as wildcard ignoring case</c>.
	/// </summary>
	/// <exception cref="System.ArgumentNullException">The <paramref name="name" /> is <see langword="null" />.</exception>
	public MatchPhrase WithName(string name)
	{
		name.ThrowIfNull();
		return new MatchPhrase(this)
		{
			Name = name,
		};
	}

	/// <summary>
	///     Interprets the expected value as a pattern of the <paramref name="kind" />, e.g. <c>"wildcard"</c>, which
	///     precedes an expected item of a collection, e.g. <c>wildcard "foo*"</c>, and which is called the
	///     <paramref name="noun" />, e.g. <c>"wildcard pattern"</c>, when the expected value is rejected.
	/// </summary>
	/// <remarks>
	///     A pattern describes what to look for, so Core rejects a <see langword="null" /> or an empty pattern before the
	///     matcher sees it.
	/// </remarks>
	/// <exception cref="System.ArgumentNullException">
	///     The <paramref name="kind" /> or the <paramref name="noun" /> is <see langword="null" />.
	/// </exception>
	public MatchPhrase WithPattern(string kind, string noun)
	{
		kind.ThrowIfNull();
		noun.ThrowIfNull();
		return new MatchPhrase(this)
		{
			PatternItemPrefix = kind + " ",
			PatternNoun = noun,
		};
	}

	/// <summary>
	///     Appends the <paramref name="suffix" /> to the expected value in the expectation text, e.g. <c>" as block"</c>
	///     in <c>matches "foo" as block</c>.
	/// </summary>
	/// <exception cref="System.ArgumentNullException">The <paramref name="suffix" /> is <see langword="null" />.</exception>
	public MatchPhrase WithSuffix(string suffix)
	{
		suffix.ThrowIfNull();
		return new MatchPhrase(this)
		{
			Suffix = suffix,
		};
	}

	/// <summary>
	///     Writes an expected string as it is, e.g. a JSON document, instead of quoting and shortening it.
	/// </summary>
	public MatchPhrase WithUnformattedExpected()
		=> new(this)
		{
			FormatsExpected = false,
		};

	/// <summary>
	///     Writes the expectation text for the already formatted <paramref name="expected" /> value in the
	///     <paramref name="grammars" />.
	/// </summary>
	/// <remarks>
	///     Without <see cref="ExpectationGrammars.Active" />, the text has no subject, e.g. <c>equal to "foo"</c>, unless
	///     the subject is always <paramref name="isActive" />.
	/// </remarks>
	internal string GetExpectation(string expected, ExpectationGrammars grammars, bool isActive)
	{
		bool active = isActive || grammars.IsActive();
		string verb = (active, grammars.IsNegated()) switch
		{
			(true, false) => grammars.Verb(_active, _activePlural),
			(false, false) => _passive,
			(true, true) => grammars.Verb(_negatedActive, _negatedActivePlural),
			(false, true) => _negated,
		};
		return verb + " " + expected + Suffix;
	}

	/// <summary>
	///     Writes the text for a single expected item, preceded by the <paramref name="itemNoun" />, e.g.
	///     <c>an item equivalent to 3</c>.
	/// </summary>
	/// <remarks>
	///     The equality is named with the <paramref name="comparison" /> of the caller, or left out without one, so that a
	///     verb which already names the item reads <c>has item equal to 3</c>.
	/// </remarks>
	internal string GetItemExpectation(string expected, string? itemNoun, string? comparison)
	{
		string? words = _isEquality ? comparison : _passive;
		return (itemNoun is null ? "" : itemNoun + " ") + (words is null ? "" : words + " ") + expected + Suffix;
	}
}
