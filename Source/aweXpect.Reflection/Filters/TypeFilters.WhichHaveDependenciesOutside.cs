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
	///     Filter for types which have at least one dependency (a type referenced in their signature) outside the
	///     allowed <paramref name="namespaces" /> (including sub-namespaces), their own namespace and framework
	///     assemblies — the positive counterpart of
	///     <see cref="WhichDependOnlyOn(Filtered.Types, IEnumerable{string})" /> for finding violators of an
	///     allowed set.
	/// </summary>
	/// <remarks>
	///     Dependencies on types whose assembly name matches one of the
	///     <see cref="AwexpectCustomization.ReflectionCustomizationValue.ExcludedAssemblyPrefixes" /> at a
	///     name-segment boundary (<c>System</c> covers <c>System.Text.Json</c>, but not
	///     <c>SystemsBiology.Core</c>) are ignored, so that framework dependencies never count as outside the
	///     allowed set. The default prefixes include <c>Microsoft</c>, so e.g. a dependency on
	///     <c>Microsoft.EntityFrameworkCore</c> is also ignored; customize the prefixes to make such a dependency
	///     count.
	/// </remarks>
	public static Filtered.Types.NamespaceDependencyOutsideFilterResult WhichHaveDependenciesOutside(
		this Filtered.Types @this, params IEnumerable<string> namespaces)
		=> new(new NamespaceDependencyOptions(namespaces),
			options => @this.Which(Filter.Suffix<Type>(
				type => type.HasDependencyNamespaceViolations(options),
				() => $"which have dependencies outside {options.Describe()} ")));

	/// <summary>
	///     Filter for types which have at least one dependency (a type referenced in their signature) outside the
	///     allowed set formed by the filtered collections of types <paramref name="target" /> and
	///     <paramref name="additional" />, their own namespace and framework assemblies — the positive counterpart
	///     of <see cref="WhichDependOnlyOn(Filtered.Types, Filtered.Types, Filtered.Types[])" /> for finding
	///     violators of an allowed set.
	/// </summary>
	/// <remarks>
	///     The target collections are resolved once per filter; a dependency is inside the allowed set when it is
	///     a member of the union of the resolved collections (by <see cref="Type" /> identity; a generic type
	///     definition in a collection matches any construction of it). A type's own namespace never counts as
	///     outside, including its sub-namespaces unless
	///     <see cref="Filtered.Types.TypeSetDependencyOutsideFilterResult.ExcludingOwnSubNamespaces" /> is used.
	///     <para />
	///     Dependencies on types whose assembly name matches one of the
	///     <see cref="AwexpectCustomization.ReflectionCustomizationValue.ExcludedAssemblyPrefixes" /> at a
	///     name-segment boundary (<c>System</c> covers <c>System.Text.Json</c>, but not
	///     <c>SystemsBiology.Core</c>) are ignored, so that framework dependencies never count as outside the
	///     allowed set. The default prefixes include <c>Microsoft</c>, so e.g. a dependency on
	///     <c>Microsoft.EntityFrameworkCore</c> is also ignored; customize the prefixes to make such a dependency
	///     count.
	/// </remarks>
	public static Filtered.Types.TypeSetDependencyOutsideFilterResult WhichHaveDependenciesOutside(
		this Filtered.Types @this, Filtered.Types target, params Filtered.Types[] additional)
		=> new(new TypeSetDependencyOptions(target, additional),
			options => @this.Which(Filter.Suffix<Type>(
				async type =>
				{
					ResolvedTypeSet allowed = await options.Resolve();
					return type.HasDependencyTypeSetViolations(allowed);
				},
				// The parentheses delimit the target description (which ends in the target's source scope,
				// e.g. "in all loaded assemblies") from the subject collection's own source suffix.
				() => $"which have dependencies outside ({options.Describe()}) ")));
}
