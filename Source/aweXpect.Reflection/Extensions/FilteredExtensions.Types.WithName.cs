using System;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class FilteredExtensions
{
	/// <summary>
	///     Filter for types with the <paramref name="expected" /> name.
	/// </summary>
	public static Filtered.Types.StringEqualityResultType WithName(this Filtered.Types @this, string expected)
	{
		StringEqualityOptions options = new();
		return new Filtered.Types.StringEqualityResultType(@this.Which(Filter.Suffix<Type>(
				type => options.AreConsideredEqual(type.Name, expected),
				() => $"with name {options.GetExpectation(expected, ExpectationGrammars.None)} ")),
			options);
	}

	/// <summary>
	///     Filter for types with the <paramref name="expected" /> namespace.
	/// </summary>
	public static Filtered.Types.StringEqualityResultType WithNamespace(this Filtered.Types @this, string expected)
	{
		StringEqualityOptions options = new();
		return new Filtered.Types.StringEqualityResultType(@this.Which(Filter.Suffix<Type>(
				type => options.AreConsideredEqual(type.Namespace, expected),
				() => $"with namespace {options.GetExpectation(expected, ExpectationGrammars.None)} ")),
			options);
	}
}
