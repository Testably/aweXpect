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
		return new Filtered.Assemblies.StringEqualityResult(@this, options => Filter.Suffix<Assembly>(
				assembly => options.Value.AreConsideredEqual(assembly.GetTargetFramework(), expected, nameof(expected)),
				() => $" targeting {options.Value.GetExpectation(expected, ExpectationGrammars.None)}"),
			new OptionsSlot<StringEqualityOptions>(StringEqualityOptions.Default));
	}

	/// <summary>
	///     Filter for assemblies that do not target the <paramref name="unexpected" /> framework (e.g. <c>net8.0</c>).
	/// </summary>
	public static Filtered.Assemblies.StringEqualityResult WhichDoNotTarget(this Filtered.Assemblies @this,
		string unexpected)
	{
		return new Filtered.Assemblies.StringEqualityResult(@this, options => Filter.Suffix<Assembly>(
				async assembly => !await options.Value.AreConsideredEqual(assembly.GetTargetFramework(), unexpected, nameof(unexpected)),
				() => $" not targeting {options.Value.GetExpectation(unexpected, ExpectationGrammars.None)}"),
			new OptionsSlot<StringEqualityOptions>(StringEqualityOptions.Default));
	}
}
