using System;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class TypeFilters
{
	/// <summary>
	///     Filters for types that are read-only structs.
	/// </summary>
	public static Filtered.Types WhichAreReadOnly(this Filtered.Types @this)
		=> @this.Which(Filter.Prefix<Type>(
			type => type.IsReadOnlyStruct(),
			"read-only "));

	/// <summary>
	///     Filters for types that are not read-only structs.
	/// </summary>
	public static Filtered.Types WhichAreNotReadOnly(this Filtered.Types @this)
		=> @this.Which(Filter.Prefix<Type>(
			type => !type.IsReadOnlyStruct(),
			"non-read-only "));
}
