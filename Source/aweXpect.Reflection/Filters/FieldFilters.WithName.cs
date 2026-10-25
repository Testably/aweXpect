using System.Reflection;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class FieldFilters
{
	/// <summary>
	///     Filter for fields with the <paramref name="expected" /> name.
	/// </summary>
	public static Filtered.Fields.StringEqualityResult WithName(this Filtered.Fields @this, string expected)
	{
		return new Filtered.Fields.StringEqualityResult(@this, options => Filter.Suffix<FieldInfo>(
				fieldInfo => options.AreConsideredEqual(fieldInfo.Name, expected),
				() => $"with name {options.GetExpectation(expected, ExpectationGrammars.None)} "),
			new OptionsSlot<StringEqualityOptions>(new StringEqualityOptions(nameof(expected))));
	}
}
