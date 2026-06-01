using System.Reflection;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class AssemblyFilters
{
	/// <summary>
	///     Filter for assemblies without the <paramref name="unexpected" /> name.
	/// </summary>
	public static Filtered.Assemblies.StringEqualityResultType WithoutName(this Filtered.Assemblies @this,
		string unexpected)
	{
		StringEqualityOptions options = new();
		return new Filtered.Assemblies.StringEqualityResultType(@this.Which(Filter.Suffix<Assembly>(
				async assembly => !await options.AreConsideredEqual(assembly.GetName().Name, unexpected),
				() => $" without name {options.GetExpectation(unexpected, ExpectationGrammars.None)}")),
			options);
	}
}
