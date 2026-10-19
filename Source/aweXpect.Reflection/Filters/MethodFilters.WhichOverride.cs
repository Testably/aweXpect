using System.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class MethodFilters
{
	/// <summary>
	///     Filters for methods that override a base class method.
	/// </summary>
	public static Filtered.Methods WhichOverride(this Filtered.Methods @this)
		=> @this.Which(Filter.Suffix<MethodInfo>(
			method => method.IsOverride(),
			"which override a base method "));

	/// <summary>
	///     Filters for methods that do not override a base class method.
	/// </summary>
	public static Filtered.Methods WhichDoNotOverride(this Filtered.Methods @this)
		=> @this.Which(Filter.Suffix<MethodInfo>(
			method => !method.IsOverride(),
			"which do not override a base method "));
}
