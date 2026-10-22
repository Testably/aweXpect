using System.Reflection;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class AssemblyFilters
{
	/// <summary>
	///     Filter for assemblies that target the <paramref name="expected" /> framework (e.g. <c>net8.0</c>).
	/// </summary>
	public static Filtered.Assemblies.StringEqualityResult WhichTarget(this Filtered.Assemblies @this,
		string expected)
	{
		StringEqualityOptions options = new(nameof(expected));
		return new Filtered.Assemblies.StringEqualityResult(@this.Which(Filter.Suffix<Assembly>(
				assembly => options.AreConsideredEqual(assembly.GetTargetFramework(), expected),
				() => $" targeting {options.GetExpectation(expected, ExpectationGrammars.None)}")),
			options);
	}

	/// <summary>
	///     Filter for assemblies that do not target the <paramref name="unexpected" /> framework (e.g. <c>net8.0</c>).
	/// </summary>
	public static Filtered.Assemblies.StringEqualityResult WhichDoNotTarget(this Filtered.Assemblies @this,
		string unexpected)
	{
		StringEqualityOptions options = new(nameof(unexpected));
		return new Filtered.Assemblies.StringEqualityResult(@this.Which(Filter.Suffix<Assembly>(
				async assembly => !await options.AreConsideredEqual(assembly.GetTargetFramework(), unexpected),
				() => $" not targeting {options.GetExpectation(unexpected, ExpectationGrammars.None)}")),
			options);
	}
}
