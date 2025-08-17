using System;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class TypeFilters
{
	/// <summary>
	///     Filters for types that are nested.
	/// </summary>
	public static Filtered.Types WhichAreNested(this Filtered.Types @this)
		=> @this.Which(Filter.Prefix<Type>(
			type => type.IsNested,
			"nested "));

	/// <summary>
	///     Filters for types that are not nested.
	/// </summary>
	public static Filtered.Types WhichAreNotNested(this Filtered.Types @this)
		=> @this.Which(Filter.Prefix<Type>(
			type => !type.IsNested,
			"non-nested "));
}
