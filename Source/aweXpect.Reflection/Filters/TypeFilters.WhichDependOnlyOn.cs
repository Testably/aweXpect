using System;
using System.Collections.Generic;
using aweXpect.Customization;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;

namespace aweXpect.Reflection;

public static partial class TypeFilters
{
	/// <summary>
	///     Filter for types which depend on (reference in their signature) only types in the
	///     <paramref name="namespaces" /> (including sub-namespaces), their own namespace or framework assemblies.
	/// </summary>
	/// <remarks>
	///     Dependencies on types whose assembly name matches one of the
	///     <see cref="AwexpectCustomization.ReflectionCustomizationValue.ExcludedAssemblyPrefixes" /> at a
	///     name-segment boundary (<c>System</c> covers <c>System.Text.Json</c>, but not
	///     <c>SystemsBiology.Core</c>) are ignored, so
	///     that framework namespaces do not have to be listed explicitly. The default prefixes include
	///     <c>Microsoft</c>, so e.g. <c>Microsoft.EntityFrameworkCore</c> is also ignored; forbid such a dependency
	///     explicitly via <c>WhichDoNotDependOn</c> or customize the prefixes.
	/// </remarks>
	public static Filtered.Types.NamespaceDependencyOnlyOnFilterResult WhichDependOnlyOn(
		this Filtered.Types @this, params IEnumerable<string> namespaces)
		=> new(new NamespaceDependencyOptions(namespaces),
			options => @this.Which(Filter.Suffix<Type>(
				type => !type.HasDependencyNamespaceViolations(options),
				() => $"which depend only on {options.Describe()} ")));
}
