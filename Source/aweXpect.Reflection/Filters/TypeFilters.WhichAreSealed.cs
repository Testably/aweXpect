using System;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class TypeFilters
{
	/// <summary>
	///     Filters for types that are sealed.
	/// </summary>
	public static Filtered.Types WhichAreSealed(this Filtered.Types @this)
		=> @this.Which(Filter.Prefix<Type>(
			type => type.IsReallySealed(),
			"sealed "));

	/// <summary>
	///     Filters for types that are not sealed.
	/// </summary>
	public static Filtered.Types WhichAreNotSealed(this Filtered.Types @this)
		=> @this.Which(Filter.Prefix<Type>(
			type => !type.IsReallySealed(),
			"non-sealed "));
}
