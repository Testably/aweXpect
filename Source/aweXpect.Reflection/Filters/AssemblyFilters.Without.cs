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
	public static Filtered.Assemblies Without<TAttribute>(this Filtered.Assemblies @this)
		where TAttribute : Attribute
		=> @this.Which(Filter.Suffix<Assembly>(
			assembly => !assembly.HasAttribute<TAttribute>(),
			() => $" without {Formatter.Format(typeof(TAttribute))}"));
}
