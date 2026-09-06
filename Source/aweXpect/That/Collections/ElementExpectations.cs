using aweXpect.Core;

namespace aweXpect;

/// <summary>
///     The expectation texts for expectations on the elements of a collection.
/// </summary>
/// <remarks>
///     Each text has to agree with the number of the subject: the plural form is used below a plural subject
///     (see <see cref="ExpectationGrammars.Plural" />), the singular form otherwise.
/// </remarks>
internal static class ElementExpectations
{
	/// <summary>
	///     …is equal to <paramref name="expected" />.
	/// </summary>
	public static string IsEqualTo(ExpectationGrammars grammars, string expected, object options)
		=> (grammars.IsPlural(), grammars.IsNegated()) switch
		{
			(true, false) => $"are equal to {expected}{options}",
			(false, false) => $"is equal to {expected}{options}",
			(true, true) => $"are not equal to {expected}{options}",
			(false, true) => $"is not equal to {expected}{options}",
		};

	/// <summary>
	///     …is equivalent to <paramref name="expected" />.
	/// </summary>
	public static string IsEquivalentTo(ExpectationGrammars grammars, string expected)
		=> (grammars.IsPlural(), grammars.IsNegated()) switch
		{
			(true, false) => $"are equivalent to {expected}",
			(false, false) => $"is equivalent to {expected}",
			(true, true) => $"are not equivalent to {expected}",
			(false, true) => $"is not equivalent to {expected}",
		};

	/// <summary>
	///     …is exactly of type <paramref name="type" />.
	/// </summary>
	public static string IsExactlyOfType(ExpectationGrammars grammars, string type)
		=> (grammars.IsPlural(), grammars.IsNegated()) switch
		{
			(true, false) => $"are exactly of type {type}",
			(false, false) => $"is exactly of type {type}",
			(true, true) => $"are not exactly of type {type}",
			(false, true) => $"is not exactly of type {type}",
		};

	/// <summary>
	///     …is of type <paramref name="type" />.
	/// </summary>
	public static string IsOfType(ExpectationGrammars grammars, string type)
		=> (grammars.IsPlural(), grammars.IsNegated()) switch
		{
			(true, false) => $"are of type {type}",
			(false, false) => $"is of type {type}",
			(true, true) => $"are not of type {type}",
			(false, true) => $"is not of type {type}",
		};

	/// <summary>
	///     …satisfies the <paramref name="predicate" />.
	/// </summary>
	public static string Satisfies(ExpectationGrammars grammars, string predicate)
		=> (grammars.IsPlural(), grammars.IsNegated()) switch
		{
			(true, false) => $"satisfy {predicate}",
			(false, false) => $"satisfies {predicate}",
			(true, true) => $"do not satisfy {predicate}",
			(false, true) => $"does not satisfy {predicate}",
		};
}
