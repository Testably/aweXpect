using System;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class TypeFilters
{
	/// <summary>
	///     Filters for types that are abstract.
	/// </summary>
	public static Filtered.Types WhichAreAbstract(this Filtered.Types @this)
		=> @this.Which(Filter.Prefix<Type>(
			type => type.IsReallyAbstract(),
			"abstract "));

	/// <summary>
	///     Filters for types that are not abstract.
	/// </summary>
	public static Filtered.Types WhichAreNotAbstract(this Filtered.Types @this)
		=> @this.Which(Filter.Prefix<Type>(
			type => !type.IsReallyAbstract(),
			"non-abstract "));
}
