using System.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class MethodFilters
{
	/// <summary>
	///     Filters for methods that are operators (e.g. <c>op_Addition</c>, <c>op_Equality</c>, …).
	/// </summary>
	public static Filtered.Methods WhichAreOperators(this Filtered.Methods @this)
		=> @this.Which(Filter.Prefix<MethodInfo>(
			method => method.IsOperator(),
			"operator "));

	/// <summary>
	///     Filters for methods that are not operators (e.g. <c>op_Addition</c>, <c>op_Equality</c>, …).
	/// </summary>
	public static Filtered.Methods WhichAreNotOperators(this Filtered.Methods @this)
		=> @this.Which(Filter.Prefix<MethodInfo>(
			method => !method.IsOperator(),
			"non-operator "));
}
