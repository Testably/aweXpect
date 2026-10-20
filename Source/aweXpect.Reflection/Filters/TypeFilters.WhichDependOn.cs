using System;
using System.Collections.Generic;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Options;

namespace aweXpect.Reflection;

public static partial class TypeFilters
{
	/// <summary>
	///     Filter for types which depend on (reference in their signature) at least one type in one of the
	///     <paramref name="namespaces" /> (including sub-namespaces).
	/// </summary>
	public static Filtered.Types.NamespaceDependencyFilterResult WhichDependOn(
		this Filtered.Types @this, params IEnumerable<string> namespaces)
		=> new(new NamespaceDependencyOptions(namespaces),
			options => @this.Which(Filter.Suffix<Type>(
				type => options.IsMatchedBy(type),
				() => $"which depend on {options.Describe()} ")));

	/// <summary>
	///     Filter for types which depend on (reference in their signature) at least one type in the filtered
	///     collections of types <paramref name="target" /> or <paramref name="additional" />.
	/// </summary>
	/// <remarks>
	///     The target collections are resolved once per filter; a dependency matches when it is a member of the
	///     union of the resolved collections (by <see cref="Type" /> identity; a generic type definition in a
	///     collection matches any construction of it).
	/// </remarks>
	public static Filtered.Types.TypeSetDependencyFilterResult WhichDependOn(
		this Filtered.Types @this, Filtered.Types target, params Filtered.Types[] additional)
		=> new(new TypeSetDependencyOptions(target, additional),
			options => @this.Which(Filter.Suffix<Type>(
				async type =>
				{
					ResolvedTypeSet targetSet = await options.Resolve();
					return targetSet.IsMatchedBy(type);
				},
				// The parentheses delimit the target description (which ends in the target's source scope,
				// e.g. "in all loaded assemblies") from the subject collection's own source suffix.
				() => $"which depend on ({options.Describe()}) ")));

	/// <summary>
	///     Filter for types which do not depend on (do not reference in their signature) any type in one of the
	///     <paramref name="namespaces" /> (including sub-namespaces).
	/// </summary>
	public static Filtered.Types.NamespaceDependencyFilterResult WhichDoNotDependOn(
		this Filtered.Types @this, params IEnumerable<string> namespaces)
		=> new(new NamespaceDependencyOptions(namespaces),
			options => @this.Which(Filter.Suffix<Type>(
				type => !options.IsMatchedBy(type),
				() => $"which do not depend on {options.Describe()} ")));

	/// <summary>
	///     Filter for types which do not depend on (do not reference in their signature) any type in the filtered
	///     collections of types <paramref name="target" /> or <paramref name="additional" />.
	/// </summary>
	/// <remarks>
	///     The target collections are resolved once per filter; a dependency matches when it is a member of the
	///     union of the resolved collections (by <see cref="Type" /> identity; a generic type definition in a
	///     collection matches any construction of it).
	/// </remarks>
	public static Filtered.Types.TypeSetDependencyFilterResult WhichDoNotDependOn(
		this Filtered.Types @this, Filtered.Types target, params Filtered.Types[] additional)
		=> new(new TypeSetDependencyOptions(target, additional),
			options => @this.Which(Filter.Suffix<Type>(
				async type =>
				{
					ResolvedTypeSet targetSet = await options.Resolve();
					return !targetSet.IsMatchedBy(type);
				},
				// The parentheses delimit the target description (which ends in the target's source scope,
				// e.g. "in all loaded assemblies") from the subject collection's own source suffix.
				() => $"which do not depend on ({options.Describe()}) ")));
}
