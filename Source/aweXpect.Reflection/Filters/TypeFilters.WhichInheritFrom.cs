using System;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class TypeFilters
{
	/// <summary>
	///     Filter for types that inherit from <typeparamref name="TBaseType" />.
	/// </summary>
	public static Filtered.Types WhichInheritFrom<TBaseType>(this Filtered.Types @this, bool forceDirect = false)
		=> @this.WhichInheritFrom(typeof(TBaseType), forceDirect);

	/// <summary>
	///     Filter for types that inherit from <paramref name="baseType" />.
	/// </summary>
	public static Filtered.Types WhichInheritFrom(this Filtered.Types @this, Type baseType, bool forceDirect = false)
		=> @this.Which(Filter.Suffix<Type>(type => type.InheritsFrom(baseType, forceDirect),
			$"which inherit from {Formatter.Format(baseType)} "));
}
