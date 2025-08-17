using System.Reflection;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class MethodFilters
{
	/// <summary>
	///     Filters for methods that are generic.
	/// </summary>
	public static Filtered.Methods WhichAreGeneric(this Filtered.Methods @this)
		=> @this.Which(Filter.Prefix<MethodInfo>(
			method => method.IsGenericMethod,
			"generic "));

	/// <summary>
	///     Filters for methods that are not generic.
	/// </summary>
	public static Filtered.Methods WhichAreNotGeneric(this Filtered.Methods @this)
		=> @this.Which(Filter.Prefix<MethodInfo>(
			method => !method.IsGenericMethod,
			"non-generic "));
}
