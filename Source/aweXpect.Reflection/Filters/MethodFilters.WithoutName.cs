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
	public static Filtered.Methods.StringEqualityResultType WithoutName(this Filtered.Methods @this, string unexpected)
	{
		StringEqualityOptions options = new();
		return new Filtered.Methods.StringEqualityResultType(@this.Which(Filter.Suffix<MethodInfo>(
				async methodInfo => !await options.AreConsideredEqual(methodInfo.Name, unexpected),
				() => $"without name {options.GetExpectation(unexpected, ExpectationGrammars.None)} ")),
			options);
	}
}
