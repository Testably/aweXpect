using System;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class TypeFilters
{
	/// <summary>
	///     Filter for types with the <paramref name="expected" /> name.
	/// </summary>
	public static Filtered.Types.StringEqualityResult WithName(this Filtered.Types @this, string expected)
	{
		return new Filtered.Types.StringEqualityResult(@this, options => Filter.Suffix<Type>(
				type => options.AreConsideredEqual(type.Name, expected, nameof(expected)),
				() => $"with name {options.GetExpectation(expected, ExpectationGrammars.None)} "),
			new OptionsSlot<StringEqualityOptions>(StringEqualityOptions.Default));
	}
}
