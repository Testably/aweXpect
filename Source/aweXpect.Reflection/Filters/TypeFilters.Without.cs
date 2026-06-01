using System;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class TypeFilters
{
	/// <summary>
	///     Filter for types without attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	/// <remarks>
	///     The optional parameter <paramref name="inherit" /> (default value <see langword="true" /> specifies, if
	///     the attribute can be inherited from a base type.
	/// </remarks>
	public static Filtered.Types Without<TAttribute>(this Filtered.Types @this, bool inherit = true)
		where TAttribute : Attribute
		=> @this.Which(Filter.Suffix<Type>(
			type => !type.HasAttribute<TAttribute>(inherit: inherit),
			$"without {(inherit ? "" : DirectText)}{Formatter.Format(typeof(TAttribute))} "));
}
