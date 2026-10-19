using System;
using System.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class AssemblyFilters
{
	/// <summary>
	///     Filter for assemblies without attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	/// <remarks>
	///     The optional parameter <paramref name="inherit" /> (default value <see langword="true" /> specifies, if
	///     the attribute can be inherited from a base type.
	/// </remarks>
	public static Filtered.Assemblies Without<TAttribute>(this Filtered.Assemblies @this, bool inherit = true)
		where TAttribute : Attribute
		=> @this.Which(Filter.Suffix<Assembly>(
			assembly => !assembly.HasAttribute<TAttribute>(inherit: inherit),
			$" without {(inherit ? "" : DirectText)}{Formatter.Format(typeof(TAttribute))}"));
}
