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
	public static Filtered.Properties.StringEqualityResult WithName(this Filtered.Properties @this, string expected)
	{
		return new Filtered.Properties.StringEqualityResult(@this, options => Filter.Suffix<PropertyInfo>(
				propertyInfo => options.AreConsideredEqual(propertyInfo.Name, expected, nameof(expected)),
				() => $"with name {options.GetExpectation(expected, ExpectationGrammars.None)} "),
			new OptionsSlot<StringEqualityOptions>(StringEqualityOptions.Default));
	}
}
