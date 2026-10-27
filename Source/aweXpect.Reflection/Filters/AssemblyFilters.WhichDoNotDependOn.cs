using System.Reflection;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class AssemblyFilters
{
	/// <summary>
	///     Filter for assemblies which have no dependency on the <paramref name="unexpected" /> assembly.
	/// </summary>
	public static Filtered.Assemblies.StringEqualityResult WhichDoNotDependOn(
		this Filtered.Assemblies @this, string unexpected)
	{
		return new Filtered.Assemblies.StringEqualityResult(@this, options => Filter.Suffix<Assembly>(
				async assembly => !await assembly.GetReferencedAssemblyNames()
					.AnyAsync(dependency => options.Value.AreConsideredEqual(dependency.Name, unexpected, nameof(unexpected))),
				() => $" which have no dependency on assembly {options.Value.GetExpectation(unexpected, ExpectationGrammars.None)}"),
			new OptionsHolder<StringEqualityOptions>(StringEqualityOptions.Default));
	}
}
