using aweXpect.Core;

namespace aweXpect.Helpers;

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
	public static string Verb(this ExpectationGrammars grammars, string singular, string plural)
		=> grammars.IsPlural() ? plural : singular;

	/// <summary>
	///     Returns the <paramref name="plural" /> form, if the result subject <paramref name="it" /> is a plural
	///     member name, otherwise the <paramref name="singular" /> form.
	/// </summary>
	/// <remarks>
	///     Unlike the expectation text, the result text is about <paramref name="it" />. As long as the subject is
	///     still the pronoun "it", the singular form is required, even for a plural expectation.
	/// </remarks>
	public static string SubjectVerb(this ExpectationGrammars grammars, string it, string singular, string plural)
		=> grammars.IsPlural() && it != SubjectPronoun ? plural : singular;
}
