using System.Reflection;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class MethodFilters
{
	/// <summary>
	///     Filter for methods without the <paramref name="unexpected" /> name.
	/// </summary>
	public static Filtered.Methods.StringEqualityResult WithoutName(this Filtered.Methods @this, string unexpected)
	{
		return new Filtered.Methods.StringEqualityResult(@this, options => Filter.Suffix<MethodInfo>(
				async methodInfo => !await options.AreConsideredEqual(methodInfo.Name, unexpected),
				() => $"without name {options.GetExpectation(unexpected, ExpectationGrammars.None)} "),
			new OptionsSlot<StringEqualityOptions>(new StringEqualityOptions(nameof(unexpected))));
	}
}
