using System;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class TypeFilters
{
	/// <summary>
	///     Filters for types that are record structs.
	/// </summary>
	public static Filtered.Types WhichAreRecordStructs(this Filtered.Types @this)
		=> @this.Which(Filter.Suffix<Type>(
			type => type.IsRecordStruct(),
			"which are record structs "));

	/// <summary>
	///     Filters for types that are not record structs.
	/// </summary>
	public static Filtered.Types WhichAreNotRecordStructs(this Filtered.Types @this)
		=> @this.Which(Filter.Suffix<Type>(
			type => !type.IsRecordStruct(),
			"which are not record structs "));
}
