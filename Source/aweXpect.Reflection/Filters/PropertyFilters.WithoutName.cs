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
		return new Filtered.Properties.StringEqualityResult(@this, options => Filter.Suffix<PropertyInfo>(
				async propertyInfo => !await options.Value.AreConsideredEqual(propertyInfo.Name, unexpected, nameof(unexpected)),
				() => $"without name {options.Value.GetExpectation(unexpected, ExpectationGrammars.None)} "),
			new OptionsSlot<StringEqualityOptions>(StringEqualityOptions.Default));
	}
}
