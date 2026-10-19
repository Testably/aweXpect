using System.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class MethodFilters
{
	/// <summary>
	///     Filters for methods that are operators (e.g. <c>op_Addition</c>, <c>op_Equality</c>, …).
	/// </summary>
	/// <remarks>
	///     This filter implicitly re-includes operator special-name members for the query, so it works without prior
	///     configuration of <c>IncludedSpecialNameMembers</c>.
	/// </remarks>
	public static Filtered.Methods WhichAreOperators(this Filtered.Methods @this)
		=> @this.WithOperatorsIncluded().Which(Filter.Prefix<MethodInfo>(
			method => method.IsOperator(),
			"operator "));

	/// <summary>
	///     Filters for methods that are the specific <paramref name="operator" />
	///     (e.g. <see cref="Operator.Addition" /> matches <c>op_Addition</c>).
	/// </summary>
	/// <remarks>
	///     This filter implicitly re-includes operator special-name members for the query, so it works without prior
	///     configuration of <c>IncludedSpecialNameMembers</c>.
	/// </remarks>
	public static Filtered.Methods WhichAreOperators(this Filtered.Methods @this, Operator @operator)
		=> @this.WithOperatorsIncluded().Which(Filter.Prefix<MethodInfo>(
			method => method.IsOperator(@operator),
			$"{OperatorNames.Display(@operator)} operator "));

	/// <summary>
	///     Filters for methods that are not operators (e.g. <c>op_Addition</c>, <c>op_Equality</c>, …).
	/// </summary>
	public static Filtered.Methods WhichAreNotOperators(this Filtered.Methods @this)
		=> @this.Which(Filter.Prefix<MethodInfo>(
			method => !method.IsOperator(),
			"non-operator "));

	/// <summary>
	///     Filters for methods that are not the specific <paramref name="operator" />
	///     (e.g. <see cref="Operator.Addition" /> matches <c>op_Addition</c>).
	/// </summary>
	/// <remarks>
	///     This filter implicitly re-includes operator special-name members for the query, so that the other operators
	///     remain in the result without prior configuration of <c>IncludedSpecialNameMembers</c>.
	/// </remarks>
	public static Filtered.Methods WhichAreNotOperators(this Filtered.Methods @this, Operator @operator)
		=> @this.WithOperatorsIncluded().Which(Filter.Prefix<MethodInfo>(
			method => !method.IsOperator(@operator),
			$"non-{OperatorNames.Display(@operator)} operator "));
}
