using System;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class TypeFilters
{
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
