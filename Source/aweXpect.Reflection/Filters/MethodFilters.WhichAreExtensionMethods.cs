using System.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class MethodFilters
{
	/// <summary>
	///     Filters for methods that are extension methods (whose first parameter is declared with the
	///     <see langword="this" /> modifier).
	/// </summary>
	public static Filtered.Methods WhichAreExtensionMethods(this Filtered.Methods @this)
		=> @this.Which(Filter.Prefix<MethodInfo>(
			method => method.IsExtensionMethod(),
			"extension "));

	/// <summary>
	///     Filters for methods that are not extension methods (whose first parameter is not declared with the
	///     <see langword="this" /> modifier).
	/// </summary>
	public static Filtered.Methods WhichAreNotExtensionMethods(this Filtered.Methods @this)
		=> @this.Which(Filter.Prefix<MethodInfo>(
			method => !method.IsExtensionMethod(),
			"non-extension "));
}
