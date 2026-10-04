namespace aweXpect.Core.Extending;

/// <summary>
///     Chooses between the singular and the plural form of a verb according to the <see cref="ExpectationGrammars" />
///     of a constraint.
/// </summary>
/// <remarks>
///     This class is in its own namespace, so that its methods are only suggested to extension authors who import it.
/// </remarks>
public static class GrammarExtensions
{
	/// <summary>
	///     The subject pronoun used while no member name replaced it.
	/// </summary>
	private const string SubjectPronoun = "it";

	/// <summary>
	///     Returns the <paramref name="plural" /> form if the <paramref name="grammars" /> have the
	///     <see cref="ExpectationGrammars.Plural" /> flag, otherwise the <paramref name="singular" /> form.
	/// </summary>
	/// <remarks>
	///     Use this for the expectation text, which is about the subject of the expectation, e.g.
	///     <c>grammars.Verb("is radio friendly", "are radio friendly")</c>.
	/// </remarks>
	public static string Verb(this ExpectationGrammars grammars, string singular, string plural)
		=> grammars.IsPlural() ? plural : singular;

	/// <summary>
	///     Returns the <paramref name="plural" /> form if the <paramref name="grammars" /> have the
	///     <see cref="ExpectationGrammars.Plural" /> flag and <paramref name="it" /> is a member name,
	///     otherwise the <paramref name="singular" /> form.
	/// </summary>
	/// <remarks>
	///     Use this for the result text, which is about <paramref name="it" />. As long as the subject is still the
	///     generic pronoun <c>it</c>, the singular form is required, even for a plural expectation, e.g.
	///     <c>grammars.SubjectVerb(It, " was", " were")</c> reads "it was" or, for a member, "Tracks were".
	/// </remarks>
	public static string SubjectVerb(this ExpectationGrammars grammars, string it, string singular, string plural)
		=> grammars.IsPlural() && it != SubjectPronoun ? plural : singular;
}
