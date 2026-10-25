using System.Reflection;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class MethodFilters
{
	/// <summary>
	///     Filter for methods with the <paramref name="expected" /> name.
	/// </summary>
	public static Filtered.Methods.StringEqualityResult WithName(this Filtered.Methods @this, string expected)
	{
		return new Filtered.Methods.StringEqualityResult(@this, options => Filter.Suffix<MethodInfo>(
				methodInfo => options.AreConsideredEqual(methodInfo.Name, expected),
				() => $"with name {options.GetExpectation(expected, ExpectationGrammars.None)} "),
			new OptionsSlot<StringEqualityOptions>(new StringEqualityOptions(nameof(expected))));
	}
}
