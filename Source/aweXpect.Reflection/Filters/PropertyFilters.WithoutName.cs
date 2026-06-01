using System.Reflection;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class PropertyFilters
{
	/// <summary>
	///     Filter for properties without the <paramref name="unexpected" /> name.
	/// </summary>
	public static Filtered.Properties.StringEqualityResultType WithoutName(this Filtered.Properties @this,
		string unexpected)
	{
		StringEqualityOptions options = new();
		return new Filtered.Properties.StringEqualityResultType(@this.Which(Filter.Suffix<PropertyInfo>(
				async propertyInfo => !await options.AreConsideredEqual(propertyInfo.Name, unexpected),
				() => $"without name {options.GetExpectation(unexpected, ExpectationGrammars.None)} ")),
			options);
	}
}
