using System;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class TypeFilters
{
	/// <summary>
	///     Filter for types without the <paramref name="unexpected" /> name.
	/// </summary>
	public static Filtered.Types.StringEqualityResultType WithoutName(this Filtered.Types @this, string unexpected)
	{
		StringEqualityOptions options = new();
		return new Filtered.Types.StringEqualityResultType(@this.Which(Filter.Suffix<Type>(
				async type => !await options.AreConsideredEqual(type.Name, unexpected),
				() => $"without name {options.GetExpectation(unexpected, ExpectationGrammars.None)} ")),
			options);
	}
}
