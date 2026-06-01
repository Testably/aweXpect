using System.Reflection;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class FieldFilters
{
	/// <summary>
	///     Filter for fields without the <paramref name="unexpected" /> name.
	/// </summary>
	public static Filtered.Fields.StringEqualityResultType WithoutName(this Filtered.Fields @this, string unexpected)
	{
		StringEqualityOptions options = new();
		return new Filtered.Fields.StringEqualityResultType(@this.Which(Filter.Suffix<FieldInfo>(
				async fieldInfo => !await options.AreConsideredEqual(fieldInfo.Name, unexpected),
				() => $"without name {options.GetExpectation(unexpected, ExpectationGrammars.None)} ")),
			options);
	}
}
