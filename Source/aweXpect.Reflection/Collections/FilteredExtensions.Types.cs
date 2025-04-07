using System;
using aweXpect.Reflection.Extensions;

namespace aweXpect.Reflection.Collections;

public static partial class FilteredExtensions
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
			$"which inherit from {baseType.Name} "));
}
