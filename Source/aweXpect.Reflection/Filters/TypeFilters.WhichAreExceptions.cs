using System;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class TypeFilters
{
	/// <summary>
	///     Filters for types that are exceptions.
	/// </summary>
	public static Filtered.Types WhichAreExceptions(this Filtered.Types @this)
		=> @this.Which(Filter.Suffix<Type>(
			type => type.IsException(),
			"which are exceptions "));

	/// <summary>
	///     Filters for types that are not exceptions.
	/// </summary>
	public static Filtered.Types WhichAreNotExceptions(this Filtered.Types @this)
		=> @this.Which(Filter.Suffix<Type>(
			type => !type.IsException(),
			"which are not exceptions "));
}
