using System;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class TypeFilters
{
	/// <summary>
	///     Filters for types that are delegates.
	/// </summary>
	public static Filtered.Types WhichAreDelegates(this Filtered.Types @this)
		=> @this.Which(Filter.Suffix<Type>(
			type => type.IsDelegate(),
			"which are delegates "));

	/// <summary>
	///     Filters for types that are not delegates.
	/// </summary>
	public static Filtered.Types WhichAreNotDelegates(this Filtered.Types @this)
		=> @this.Which(Filter.Suffix<Type>(
			type => !type.IsDelegate(),
			"which are not delegates "));
}
