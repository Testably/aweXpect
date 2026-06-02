using System.Reflection;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class MethodFilters
{
	/// <summary>
	///     Filters for methods that are virtual.
	/// </summary>
	public static Filtered.Methods WhichAreVirtual(this Filtered.Methods @this)
		=> @this.Which(Filter.Prefix<MethodInfo>(
			method => method.IsVirtual,
			"virtual "));

	/// <summary>
	///     Filters for methods that are not virtual.
	/// </summary>
	public static Filtered.Methods WhichAreNotVirtual(this Filtered.Methods @this)
		=> @this.Which(Filter.Prefix<MethodInfo>(
			method => !method.IsVirtual,
			"non-virtual "));
}
