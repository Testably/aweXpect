using System;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class TypeFilters
{
	/// <summary>
	///     Filters for types that are ref structs.
	/// </summary>
	public static Filtered.Types WhichAreRefStructs(this Filtered.Types @this)
		=> @this.Which(Filter.Suffix<Type>(
			type => type.IsRefStruct(),
			"which are ref structs "));

	/// <summary>
	///     Filters for types that are not ref structs.
	/// </summary>
	public static Filtered.Types WhichAreNotRefStructs(this Filtered.Types @this)
		=> @this.Which(Filter.Suffix<Type>(
			type => !type.IsRefStruct(),
			"which are not ref structs "));
}
