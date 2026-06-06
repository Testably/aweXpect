using System.Linq;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class AssemblyFilters
{
	/// <summary>
	///     Filter for assemblies which have a dependency on the <paramref name="expected" /> assembly.
	/// </summary>
	public static Filtered.Assemblies.StringEqualityResultType WhichDependOn(
		this Filtered.Assemblies @this, string expected)
	{
		StringEqualityOptions options = new();
		return new Filtered.Assemblies.StringEqualityResultType(@this.Which(Filter.Suffix<Assembly>(
				assembly => assembly.GetReferencedAssemblies()
					.AnyAsync(dependency => options.AreConsideredEqual(dependency.Name, expected)),
				() => $" which have a dependency on assembly {options.GetExpectation(expected, ExpectationGrammars.None)}")),
			options);
	}
}
