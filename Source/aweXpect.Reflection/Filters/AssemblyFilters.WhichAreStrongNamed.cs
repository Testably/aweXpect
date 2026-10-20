using System.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class AssemblyFilters
{
	/// <summary>
	///     Filter for assemblies that are strong named.
	/// </summary>
	public static Filtered.Assemblies WhichAreStrongNamed(this Filtered.Assemblies @this)
		=> @this.Which(Filter.Suffix<Assembly>(
			assembly => assembly.IsStrongNamed(),
			" which are strong named"));

	/// <summary>
	///     Filter for assemblies that are not strong named.
	/// </summary>
	public static Filtered.Assemblies WhichAreNotStrongNamed(this Filtered.Assemblies @this)
		=> @this.Which(Filter.Suffix<Assembly>(
			assembly => !assembly.IsStrongNamed(),
			" which are not strong named"));
}
