using System.Reflection;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class PropertyFilters
{
	/// <summary>
	///     Filter for properties with the <paramref name="expected" /> name.
	/// </summary>
	public static Filtered.Properties.StringEqualityResultType WithName(this Filtered.Properties @this, string expected)
	{
		StringEqualityOptions options = new();
		return new Filtered.Properties.StringEqualityResultType(@this.Which(Filter.Suffix<PropertyInfo>(
				propertyInfo => options.AreConsideredEqual(propertyInfo.Name, expected),
				() => $"with name {options.GetExpectation(expected, ExpectationGrammars.None)} ")),
			options);
	}
}
