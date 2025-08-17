using System.Reflection;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class MethodFilters
{
	/// <summary>
	///     Filters for methods that are static.
	/// </summary>
	public static Filtered.Methods WhichAreStatic(this Filtered.Methods @this)
		=> @this.Which(Filter.Prefix<MethodInfo>(
			method => method.IsStatic,
			"static "));

	/// <summary>
	///     Filters for methods that are not static.
	/// </summary>
	public static Filtered.Methods WhichAreNotStatic(this Filtered.Methods @this)
		=> @this.Which(Filter.Prefix<MethodInfo>(
			method => !method.IsStatic,
			"non-static "));
}
