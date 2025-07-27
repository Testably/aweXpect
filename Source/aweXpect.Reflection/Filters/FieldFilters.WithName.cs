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
	public static Filtered.Fields.StringEqualityResultType WithName(this Filtered.Fields @this, string expected)
	{
		StringEqualityOptions options = new();
		return new Filtered.Fields.StringEqualityResultType(@this.Which(Filter.Suffix<FieldInfo>(
				fieldInfo => options.AreConsideredEqual(fieldInfo.Name, expected),
				() => $"with name {options.GetExpectation(expected, ExpectationGrammars.None)} ")),
			options);
	}
}
