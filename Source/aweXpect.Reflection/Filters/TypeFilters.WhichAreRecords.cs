using System;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class TypeFilters
{
	/// <summary>
	///     Filters for types that are records.
	/// </summary>
	public static Filtered.Types WhichAreRecords(this Filtered.Types @this)
		=> @this.Which(Filter.Suffix<Type>(
			type => type.IsRecordClass(),
			"which are records "));

	/// <summary>
	///     Filters for types that are not records.
	/// </summary>
	public static Filtered.Types WhichAreNotRecords(this Filtered.Types @this)
		=> @this.Which(Filter.Suffix<Type>(
			type => !type.IsRecordClass(),
			"which are not records "));
}
