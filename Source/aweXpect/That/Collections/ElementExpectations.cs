using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect;

/// <summary>
///     The expectation texts for expectations on the elements of a collection.
/// </summary>
/// <remarks>
///     Each text has to agree with the number of the subject: the plural form is used below a plural subject
///     (see <see cref="ExpectationGrammars.Plural" />), the singular form otherwise.<br />
///     The quantifier of a collection expectation carries its negation, so only the uniqueness texts, which can
///     expect an item not to be unique, have a negated form.
/// </remarks>
internal static class ElementExpectations
{
	/// <summary>
	///     …is equal to <paramref name="expected" />.
	/// </summary>
	public static string IsEqualTo(ExpectationGrammars grammars, string expected, object options)
		=> grammars.IsPlural()
			? $"are equal to {expected}{options}"
			: $"is equal to {expected}{options}";

	/// <summary>
	///     …is equal to the <paramref name="expected" /> string, or matches it as the pattern of the
	///     <paramref name="options" />.
	/// </summary>
	/// <remarks>
	///     A pattern describes the item, so it is named before the value like on a single string. An exact match is
	///     described by the <paramref name="displayedOptions" />, which can also name the comparer of the subject.
	/// </remarks>
	public static string IsEqualToString(ExpectationGrammars grammars, string? expected,
		StringEqualityOptions options, object? displayedOptions = null)
		=> options.InspectsSubject
			? options.GetExpectation(expected, grammars | ExpectationGrammars.Active)
			: IsEqualTo(grammars, Formatter.Format(expected), displayedOptions ?? options);

	/// <summary>
	///     …is equivalent to <paramref name="expected" />.
	/// </summary>
	public static string IsEquivalentTo(ExpectationGrammars grammars, string expected)
		=> grammars.IsPlural()
			? $"are equivalent to {expected}"
			: $"is equivalent to {expected}";

	/// <summary>
	///     …is exactly of type <paramref name="type" />.
	/// </summary>
	public static string IsExactlyOfType(ExpectationGrammars grammars, string type)
		=> grammars.IsPlural()
			? $"are exactly of type {type}"
			: $"is exactly of type {type}";

	/// <summary>
	///     …is of type <paramref name="type" />.
	/// </summary>
	public static string IsOfType(ExpectationGrammars grammars, string type)
		=> grammars.IsPlural()
			? $"are of type {type}"
			: $"is of type {type}";

	/// <summary>
	///     …is unique within the collection.
	/// </summary>
	public static string IsUnique(ExpectationGrammars grammars, object options)
		=> (grammars.IsPlural(), grammars.IsNegated()) switch
		{
			(true, false) => $"are unique{options}",
			(false, false) => $"is unique{options}",
			(true, true) => $"are not unique{options}",
			(false, true) => $"is not unique{options}",
		};

	/// <summary>
	///     …is unique within the collection for the <paramref name="memberAccessor" />.
	/// </summary>
	public static string IsUniqueFor(ExpectationGrammars grammars, string memberAccessor, object options)
		=> (grammars.IsPlural(), grammars.IsNegated()) switch
		{
			(true, false) => $"are unique by {memberAccessor}{options}",
			(false, false) => $"is unique by {memberAccessor}{options}",
			(true, true) => $"are not unique by {memberAccessor}{options}",
			(false, true) => $"is not unique by {memberAccessor}{options}",
		};

	/// <summary>
	///     …satisfies the <paramref name="predicate" />.
	/// </summary>
	public static string Satisfies(ExpectationGrammars grammars, string predicate)
		=> grammars.IsPlural()
			? $"satisfy {predicate}"
			: $"satisfies {predicate}";
}
