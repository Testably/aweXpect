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
	public static Filtered.Properties.StringEqualityResult WithoutName(this Filtered.Properties @this,
		string unexpected)
	{
		StringEqualityOptions options = new(nameof(unexpected));
		return new Filtered.Properties.StringEqualityResult(@this.Which(Filter.Suffix<PropertyInfo>(
				async propertyInfo => !await options.AreConsideredEqual(propertyInfo.Name, unexpected),
				() => $"without name {options.GetExpectation(unexpected, ExpectationGrammars.None)} ")),
			options);
	}
}
