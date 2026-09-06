namespace aweXpect.Core.Helpers;

/// <remarks>
///     This class is duplicated in the <c>aweXpect</c> assembly, because outside of the <c>Debug</c> configuration
///     it is compiled against the released <c>aweXpect.Core</c> package, which does not know about new members yet.
/// </remarks>
internal static class GrammarHelpers
{
	/// <summary>
	///     The subject pronoun used while no member name replaced it.
	/// </summary>
	private const string SubjectPronoun = "it";

	/// <summary>
	///     Returns the <paramref name="plural" /> form, if the <paramref name="grammars" /> have the
	///     <see cref="ExpectationGrammars.Plural" /> flag, otherwise the <paramref name="singular" /> form.
	/// </summary>
	/// <remarks>
	///     Use this for the expectation text, which is about the subject of the expectation.
	/// </remarks>
	public static string Verb(this ExpectationGrammars grammars, string singular, string plural)
		=> grammars.IsPlural() ? plural : singular;

	/// <summary>
	///     Returns the <paramref name="plural" /> form, if the <paramref name="grammars" /> have the
	///     <see cref="ExpectationGrammars.Plural" /> flag and <paramref name="it" /> is a member name,
	///     otherwise the <paramref name="singular" /> form.
	/// </summary>
	/// <remarks>
	///     Use this for the result text, which is about <paramref name="it" />. As long as the subject is still the
	///     generic pronoun <c>it</c>, the singular form is required, even for a plural expectation.
	/// </remarks>
	public static string SubjectVerb(this ExpectationGrammars grammars, string it, string singular, string plural)
		=> grammars.IsPlural() && it != SubjectPronoun ? plural : singular;
}
