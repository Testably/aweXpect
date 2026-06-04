using System.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class MethodFilters
{
	/// <summary>
	///     Filters for methods that are asynchronous (declared with the <see langword="async" /> keyword).
	/// </summary>
	public static Filtered.Methods WhichAreAsync(this Filtered.Methods @this)
		=> @this.Which(Filter.Prefix<MethodInfo>(
			method => method.IsAsync(),
			"async "));

	/// <summary>
	///     Filters for methods that are not asynchronous (not declared with the <see langword="async" /> keyword).
	/// </summary>
	public static Filtered.Methods WhichAreNotAsync(this Filtered.Methods @this)
		=> @this.Which(Filter.Prefix<MethodInfo>(
			method => !method.IsAsync(),
			"non-async "));
}
