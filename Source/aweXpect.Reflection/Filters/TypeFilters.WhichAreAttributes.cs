using System;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class TypeFilters
{
	/// <summary>
	///     Filters for types that are attributes.
	/// </summary>
	public static Filtered.Types WhichAreAttributes(this Filtered.Types @this)
		=> @this.Which(Filter.Suffix<Type>(
			type => type.IsAttribute(),
			"which are attributes "));

	/// <summary>
	///     Filters for types that are not attributes.
	/// </summary>
	public static Filtered.Types WhichAreNotAttributes(this Filtered.Types @this)
		=> @this.Which(Filter.Suffix<Type>(
			type => !type.IsAttribute(),
			"which are not attributes "));
}
