using System.Linq;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class AssemblyFilters
{
	/// <summary>
	///     Filter for assemblies which have no dependency on the <paramref name="unexpected" /> assembly.
	/// </summary>
	public static Filtered.Assemblies.StringEqualityResultType WhichDoNotDependOn(
		this Filtered.Assemblies @this, string unexpected)
	{
		StringEqualityOptions options = new();
		return new Filtered.Assemblies.StringEqualityResultType(@this.Which(Filter.Suffix<Assembly>(
				async assembly => !await assembly.GetReferencedAssemblies()
					.AnyAsync(dependency => options.AreConsideredEqual(dependency.Name, unexpected)),
				() => $" which have no dependency on assembly {options.GetExpectation(unexpected, ExpectationGrammars.None)}")),
			options);
	}
}
