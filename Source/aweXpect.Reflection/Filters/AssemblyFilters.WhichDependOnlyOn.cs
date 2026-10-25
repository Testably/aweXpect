using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Customization;
using aweXpect.Options;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class AssemblyFilters
{
	/// <summary>
	///     Filter for assemblies which have dependencies only on the <paramref name="expected" /> assemblies.
	/// </summary>
	/// <remarks>
	///     References to assemblies whose name matches one of the
	///     <see cref="AwexpectCustomization.SettingsCustomization.ExcludedAssemblyPrefixes" /> at a
	///     name-segment boundary (<c>System</c> covers <c>System.Text.Json</c>, but not
	///     <c>SystemsBiology.Core</c>) are ignored,
	///     so that framework assemblies do not have to be listed explicitly.
	/// </remarks>
	public static Filtered.Assemblies.StringEqualityResult WhichDependOnlyOn(
		this Filtered.Assemblies @this, params string[] expected)
		=> @this.WhichDependOnlyOn(expected.AsEnumerable());

	/// <inheritdoc cref="WhichDependOnlyOn(Filtered.Assemblies, string[])" />
	public static Filtered.Assemblies.StringEqualityResult WhichDependOnlyOn(
		this Filtered.Assemblies @this, IEnumerable<string> expected)
	{
		ThrowHelper.ThrowIfNull(expected, nameof(expected));
		string[] allowed = expected.ToArray();
		return new Filtered.Assemblies.StringEqualityResult(@this, options => Filter.Suffix<Assembly>(
				async assembly
					=> (await assembly.GetDisallowedAssemblyDependencies(allowed, options)).Length == 0,
				() => allowed.Length == 0
					? " which have dependencies only on no assemblies"
					: $" which have dependencies only on assemblies {string.Join(" or ", allowed.Select(expected => options.GetExpectation(expected, ExpectationGrammars.None)))}"),
			new StringEqualityOptions(nameof(expected)));
	}
}
