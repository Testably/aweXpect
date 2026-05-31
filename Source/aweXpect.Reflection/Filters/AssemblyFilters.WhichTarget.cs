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
	public static Filtered.Assemblies.StringEqualityResultType WhichTarget(this Filtered.Assemblies @this,
		string expected)
	{
		StringEqualityOptions options = new();
		return new Filtered.Assemblies.StringEqualityResultType(@this.Which(Filter.Suffix<Assembly>(
				assembly => options.AreConsideredEqual(assembly.GetTargetFramework(), expected),
				() => $" targeting {options.GetExpectation(expected, ExpectationGrammars.None)}")),
			options);
	}
}
