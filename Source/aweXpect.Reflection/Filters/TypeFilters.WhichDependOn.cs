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
	///     Filter for types which do not depend on (do not reference in their signature) any type in one of the
	///     <paramref name="namespaces" /> (including sub-namespaces).
	/// </summary>
	public static Filtered.Types.NamespaceDependencyFilterResult WhichDoNotDependOn(
		this Filtered.Types @this, params IEnumerable<string> namespaces)
		=> new(new NamespaceDependencyOptions(namespaces),
			options => @this.Which(Filter.Suffix<Type>(
				type => !options.IsMatchedBy(type),
				() => $"which do not depend on {options.Describe()} ")));
}
