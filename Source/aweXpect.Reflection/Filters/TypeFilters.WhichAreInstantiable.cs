using System;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class TypeFilters
{
	/// <summary>
	///     Filters for types that are instantiable.
	/// </summary>
	/// <remarks>
	///     Abstract types, static types, interfaces and open generic type definitions are not considered instantiable.
	/// </remarks>
	public static Filtered.Types WhichAreInstantiable(this Filtered.Types @this)
		=> @this.Which(Filter.Prefix<Type>(
			type => type.IsReallyInstantiable(),
			"instantiable "));

	/// <summary>
	///     Filters for types that are not instantiable.
	/// </summary>
	/// <remarks>
	///     Abstract types, static types, interfaces and open generic type definitions are not considered instantiable.
	/// </remarks>
	public static Filtered.Types WhichAreNotInstantiable(this Filtered.Types @this)
		=> @this.Which(Filter.Prefix<Type>(
			type => !type.IsReallyInstantiable(),
			"non-instantiable "));
}
