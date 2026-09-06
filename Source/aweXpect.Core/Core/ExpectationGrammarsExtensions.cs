using System.Linq;
using System.Runtime.CompilerServices;

namespace aweXpect.Core;

/// <summary>
///     Extension methods on <see cref="ExpectationGrammars" />.
/// </summary>
public static class ExpectationGrammarsExtensions
{
	/// <summary>
	///     Toggles the <see cref="ExpectationGrammars.Negated" /> flag.
	/// </summary>
	public static ExpectationGrammars Negate(this ExpectationGrammars grammars)
		=> grammars ^ ExpectationGrammars.Negated;

	/// <summary>
	///     Checks if the <paramref name="grammars" /> has the <see cref="ExpectationGrammars.Negated" /> flag.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsNegated(this ExpectationGrammars grammars)
		=> grammars.HasFlag(ExpectationGrammars.Negated);

	/// <summary>
	///     Checks if the <paramref name="grammars" /> has the <see cref="ExpectationGrammars.Nested" /> flag.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsNested(this ExpectationGrammars grammars)
		=> grammars.HasFlag(ExpectationGrammars.Nested);

	/// <summary>
	///     Checks if the <paramref name="grammars" /> has the <see cref="ExpectationGrammars.Plural" /> flag.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsPlural(this ExpectationGrammars grammars)
		=> grammars.HasFlag(ExpectationGrammars.Plural);

	/// <summary>
	///     Checks if the <paramref name="grammars" /> has any of the given <paramref name="flags" />.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool HasAnyFlag(this ExpectationGrammars grammars, params ExpectationGrammars[] flags)
		=> flags.Any(flag => grammars.HasFlag(flag));

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
		=> grammars.IsPlural() && it != ExpectationBuilder.DefaultCurrentSubject ? plural : singular;
}
