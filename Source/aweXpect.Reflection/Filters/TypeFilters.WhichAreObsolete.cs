using System;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class TypeFilters
{
	/// <summary>
	///     Filters for types that are obsolete (marked with the <see cref="ObsoleteAttribute" />).
	/// </summary>
	public static Filtered.Types WhichAreObsolete(this Filtered.Types @this)
		=> @this.Which(Filter.Prefix<Type>(
			type => type.IsObsolete(),
			"obsolete "));

	/// <summary>
	///     Filters for types that are not obsolete (not marked with the <see cref="ObsoleteAttribute" />).
	/// </summary>
	public static Filtered.Types WhichAreNotObsolete(this Filtered.Types @this)
		=> @this.Which(Filter.Prefix<Type>(
			type => !type.IsObsolete(),
			"non-obsolete "));
}
