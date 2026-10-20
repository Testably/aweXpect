using System;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class TypeFilters
{
	/// <summary>
	///     Filters for types that have an accessible parameterless (default) constructor.
	/// </summary>
	public static Filtered.Types WhichHaveADefaultConstructor(this Filtered.Types @this)
		=> @this.Which(Filter.Suffix<Type>(
			type => type.HasDefaultConstructor(),
			"which have a default constructor "));

	/// <summary>
	///     Filters for types that do not have an accessible parameterless (default) constructor.
	/// </summary>
	public static Filtered.Types WhichDoNotHaveADefaultConstructor(this Filtered.Types @this)
		=> @this.Which(Filter.Suffix<Type>(
			type => !type.HasDefaultConstructor(),
			"which do not have a default constructor "));
}
