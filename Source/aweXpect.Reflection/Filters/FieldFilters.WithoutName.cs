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
	public static Filtered.Fields.StringEqualityResult WithoutName(this Filtered.Fields @this, string unexpected)
	{
		return new Filtered.Fields.StringEqualityResult(@this, options => Filter.Suffix<FieldInfo>(
				async fieldInfo => !await options.Value.AreConsideredEqual(fieldInfo.Name, unexpected, nameof(unexpected)),
				() => $"without name {options.Value.GetExpectation(unexpected, ExpectationGrammars.None)} "),
			new OptionsSlot<StringEqualityOptions>(StringEqualityOptions.Default));
	}
}
