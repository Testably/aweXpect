using System.Reflection;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class MethodFilters
{
	/// <summary>
	///     Filters for methods that are sealed.
	/// </summary>
	public static Filtered.Methods WhichAreSealed(this Filtered.Methods @this)
		=> @this.Which(Filter.Prefix<MethodInfo>(
			method => method.IsFinal,
			"sealed "));

	/// <summary>
	///     Filters for methods that are not sealed.
	/// </summary>
	public static Filtered.Methods WhichAreNotSealed(this Filtered.Methods @this)
		=> @this.Which(Filter.Prefix<MethodInfo>(
			method => !method.IsFinal,
			"non-sealed "));
}
