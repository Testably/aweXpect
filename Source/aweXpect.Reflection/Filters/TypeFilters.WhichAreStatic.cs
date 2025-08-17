using System;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class TypeFilters
{
	/// <summary>
	///     Filters for types that are static.
	/// </summary>
	public static Filtered.Types WhichAreStatic(this Filtered.Types @this)
		=> @this.Which(Filter.Prefix<Type>(
			type => type.IsReallyStatic(),
			"static "));

	/// <summary>
	///     Filters for types that are not static.
	/// </summary>
	public static Filtered.Types WhichAreNotStatic(this Filtered.Types @this)
		=> @this.Which(Filter.Prefix<Type>(
			type => !type.IsReallyStatic(),
			"non-static "));
}
