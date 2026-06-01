using System;
using System.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class PropertyFilters
{
	/// <summary>
	///     Filter for properties without attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	/// <remarks>
	///     The optional parameter <paramref name="inherit" /> (default value <see langword="true" /> specifies, if
	///     the attribute can be inherited from a base type.
	/// </remarks>
	public static Filtered.Properties Without<TAttribute>(this Filtered.Properties @this, bool inherit = true)
		where TAttribute : Attribute
		=> @this.Which(Filter.Suffix<PropertyInfo>(
			propertyInfo => !propertyInfo.HasAttribute<TAttribute>(inherit: inherit),
			$"without {(inherit ? "" : DirectText)}{Formatter.Format(typeof(TAttribute))} "));
}
