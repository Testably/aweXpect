using System;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class TypeFilters
{
	/// <summary>
	///     Filters for types that are classes.
	/// </summary>
	public static Filtered.Types WhichAreClasses(this Filtered.Types @this)
		=> @this.Which(Filter.Suffix<Type>(
			type => type.IsReallyClass(),
			"which are classes "));

	/// <summary>
	///     Filters for types that are not classes.
	/// </summary>
	public static Filtered.Types WhichAreNotClasses(this Filtered.Types @this)
		=> @this.Which(Filter.Suffix<Type>(
			type => !type.IsReallyClass(),
			"which are not classes "));
}
