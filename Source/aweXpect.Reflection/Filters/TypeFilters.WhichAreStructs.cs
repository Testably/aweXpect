using System;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class TypeFilters
{
	/// <summary>
	///     Filters for types that are structs.
	/// </summary>
	public static Filtered.Types WhichAreStructs(this Filtered.Types @this)
		=> @this.Which(Filter.Suffix<Type>(
			type => type.IsReallyStruct(),
			"which are structs "));

	/// <summary>
	///     Filters for types that are not structs.
	/// </summary>
	public static Filtered.Types WhichAreNotStructs(this Filtered.Types @this)
		=> @this.Which(Filter.Suffix<Type>(
			type => !type.IsReallyStruct(),
			"which are not structs "));
}
