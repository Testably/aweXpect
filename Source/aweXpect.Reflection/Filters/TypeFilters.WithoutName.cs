using System;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class TypeFilters
{
	/// <summary>
	///     Filter for types without the <paramref name="unexpected" /> name.
	/// </summary>
	public static Filtered.Types.StringEqualityResult WithoutName(this Filtered.Types @this, string unexpected)
	{
		return new Filtered.Types.StringEqualityResult(@this, options => Filter.Suffix<Type>(
				async type => !await options.Value.AreConsideredEqual(type.Name, unexpected, nameof(unexpected)),
				() => $"without name {options.Value.GetExpectation(unexpected, ExpectationGrammars.None)} "),
			new OptionsHolder<StringEqualityOptions>(StringEqualityOptions.Default));
	}
}
