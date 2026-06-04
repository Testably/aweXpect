using System;
using System.Linq;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Customization;
using aweXpect.Options;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class AssemblyFilters
{
	/// <summary>
	///     Filter for assemblies which have dependencies only on the <paramref name="allowed" /> assemblies.
	/// </summary>
	/// <remarks>
	///     References to assemblies whose name starts with one of the
	///     <see cref="AwexpectCustomization.ReflectionCustomizationValue.ExcludedAssemblyPrefixes" /> are ignored,
	///     so that framework assemblies do not have to be listed explicitly.
	/// </remarks>
	public static Filtered.Assemblies.StringEqualityResultType WhichDependOnlyOn(
		this Filtered.Assemblies @this, params string[] allowed)
	{
		StringEqualityOptions options = new();
		return new Filtered.Assemblies.StringEqualityResultType(@this.Which(Filter.Suffix<Assembly>(
				assembly =>
				{
					string[] prefixes = Customize.aweXpect.Reflection().ExcludedAssemblyPrefixes.Get();
					return assembly.GetReferencedAssemblies().AllAsync(async dependency =>
						prefixes.Any(prefix => dependency.Name?.StartsWith(prefix, StringComparison.Ordinal) == true) ||
						await allowed.AnyAsync(expected => options.AreConsideredEqual(dependency.Name, expected)));
				},
				() => allowed.Length == 0
					? " which have dependencies only on no assemblies"
					: $" which have dependencies only on assemblies {string.Join(" or ", allowed.Select(expected => options.GetExpectation(expected, ExpectationGrammars.None)))}")),
			options);
	}
}
