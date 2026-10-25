using System.Linq;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class AssemblyFilters
{
	/// <summary>
	///     Filter for assemblies which have a dependency on the <paramref name="expected" /> assembly.
	/// </summary>
	public static Filtered.Assemblies.StringEqualityResult WhichDependOn(
		this Filtered.Assemblies @this, string expected)
	{
		return new Filtered.Assemblies.StringEqualityResult(@this, options => Filter.Suffix<Assembly>(
				assembly => assembly.GetReferencedAssemblyNames()
					.AnyAsync(dependency => options.AreConsideredEqual(dependency.Name, expected)),
				() => $" which have a dependency on assembly {options.GetExpectation(expected, ExpectationGrammars.None)}"),
			new OptionsSlot<StringEqualityOptions>(new StringEqualityOptions(nameof(expected))));
	}
}
