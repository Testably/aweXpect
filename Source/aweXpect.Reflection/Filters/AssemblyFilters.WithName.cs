using System.Reflection;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class AssemblyFilters
{
	/// <summary>
	///     Filter for assemblies with the <paramref name="expected" /> name.
	/// </summary>
	public static Filtered.Assemblies.StringEqualityResult WithName(this Filtered.Assemblies @this, string expected)
	{
		return new Filtered.Assemblies.StringEqualityResult(@this, options => Filter.Suffix<Assembly>(
				assembly => options.AreConsideredEqual(assembly.GetName().Name, expected),
				() => $" with name {options.GetExpectation(expected, ExpectationGrammars.None)}"),
			new StringEqualityOptions(nameof(expected)));
	}
}
