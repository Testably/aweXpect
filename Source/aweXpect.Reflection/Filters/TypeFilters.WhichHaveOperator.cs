using System;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class TypeFilters
{
	/// <summary>
	///     Filter for types which have the <paramref name="operator" />
	///     (e.g. <see cref="Operator.Addition" /> matches <c>op_Addition</c>).
	/// </summary>
	/// <remarks>
	///     Operators inherited from base types are considered as well; restrict the filter to the operators declared
	///     on the type with <see cref="InheritanceScopeResult.Directly" />.
	/// </remarks>
	public static InheritanceScopeResult WhichHaveOperator(this Filtered.Types @this, Operator @operator)
		=> new(@this, memberScope => OperatorFilter(@operator, null, memberScope.IsDirect(), true));

	/// <summary>
	///     Filter for types which have the <paramref name="operator" /> with an overload that takes the operand
	///     <typeparamref name="TOperand" /> as one of its parameters.
	/// </summary>
	/// <remarks>
	///     Operators inherited from base types are considered as well; restrict the filter to the operators declared
	///     on the type with <see cref="InheritanceScopeResult.Directly" />.
	/// </remarks>
	public static InheritanceScopeResult WhichHaveOperator<TOperand>(this Filtered.Types @this, Operator @operator)
		=> @this.WhichHaveOperator(@operator, typeof(TOperand));

	/// <summary>
	///     Filter for types which have the <paramref name="operator" /> with an overload that takes the
	///     <paramref name="operand" /> as one of its parameters.
	/// </summary>
	/// <remarks>
	///     Operators inherited from base types are considered as well; restrict the filter to the operators declared
	///     on the type with <see cref="InheritanceScopeResult.Directly" />.
	/// </remarks>
	public static InheritanceScopeResult WhichHaveOperator(this Filtered.Types @this, Operator @operator,
		Type operand)
		=> new(@this, memberScope => OperatorFilter(@operator, operand, memberScope.IsDirect(), true));

	/// <summary>
	///     Filter for types which do not have the <paramref name="operator" />
	///     (e.g. <see cref="Operator.Addition" /> matches <c>op_Addition</c>).
	/// </summary>
	/// <remarks>
	///     Operators inherited from base types are considered as well; restrict the filter to the operators declared
	///     on the type with <see cref="InheritanceScopeResult.Directly" />.
	/// </remarks>
	public static InheritanceScopeResult WhichDoNotHaveOperator(this Filtered.Types @this, Operator @operator)
		=> new(@this, memberScope => OperatorFilter(@operator, null, memberScope.IsDirect(), false));

	/// <summary>
	///     Filter for types which do not have the <paramref name="operator" /> with an overload that takes the operand
	///     <typeparamref name="TOperand" /> as one of its parameters.
	/// </summary>
	/// <remarks>
	///     Operators inherited from base types are considered as well; restrict the filter to the operators declared
	///     on the type with <see cref="InheritanceScopeResult.Directly" />.
	/// </remarks>
	public static InheritanceScopeResult WhichDoNotHaveOperator<TOperand>(this Filtered.Types @this,
		Operator @operator)
		=> @this.WhichDoNotHaveOperator(@operator, typeof(TOperand));

	/// <summary>
	///     Filter for types which do not have the <paramref name="operator" /> with an overload that takes the
	///     <paramref name="operand" /> as one of its parameters.
	/// </summary>
	/// <remarks>
	///     Operators inherited from base types are considered as well; restrict the filter to the operators declared
	///     on the type with <see cref="InheritanceScopeResult.Directly" />.
	/// </remarks>
	public static InheritanceScopeResult WhichDoNotHaveOperator(this Filtered.Types @this, Operator @operator,
		Type operand)
		=> new(@this, memberScope => OperatorFilter(@operator, operand, memberScope.IsDirect(), false));

	private static Filter.GenericFilter<Type> OperatorFilter(Operator @operator, Type? operand, bool directly, bool expected)
		=> Filter.Suffix<Type>(
			type => (operand is null
				? type.HasOperator(@operator, !directly)
				: type.HasOperator(@operator, operand, !directly)) == expected,
			() => $"which {(expected ? "" : "do not ")}{(directly ? "declare" : "have")} the operator {OperatorNames.Display(@operator)}{(operand is null ? "" : $" with operand {Formatter.Format(operand)}")} ");
}
