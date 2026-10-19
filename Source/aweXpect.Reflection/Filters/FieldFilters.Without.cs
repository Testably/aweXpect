using System;
using System.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class FieldFilters
{
	/// <summary>
	///     Filter for fields without attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	public static Filtered.Fields Without<TAttribute>(this Filtered.Fields @this)
		where TAttribute : Attribute
		=> @this.Which(Filter.Suffix<FieldInfo>(
			fieldInfo => !fieldInfo.HasAttribute<TAttribute>(),
			$"without {Formatter.Format(typeof(TAttribute))} "));
}
