using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Extending;
using aweXpect.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Results;

namespace aweXpect;

public static partial class ThatTypes
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> have the
	///     <paramref name="operator" /> (e.g. <see cref="Operator.Addition" /> matches <c>op_Addition</c>).
	/// </summary>
	/// <remarks>
	///     Operators inherited from base types are considered as well; restrict the check to the operators declared
	///     on the type with <see cref="InheritanceScopeResult{TType, TValue}.Directly" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IEnumerable<Type?>, IEnumerable<Type?>?> HaveOperator(
		this IThat<IEnumerable<Type?>?> subject,
		Operator @operator)
	{
		HaveOperatorExpectation expectation = new(@operator, null);
		return new(subject.ExpectAll(expectation), subject, expectation.Scope);
	}

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> have the
	///     <paramref name="operator" /> with an overload that takes the <paramref name="operand" />.
	/// </summary>
	/// <remarks>
	///     Operators inherited from base types are considered as well; restrict the check to the operators declared
	///     on the type with <see cref="InheritanceScopeResult{TType, TValue}.Directly" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IEnumerable<Type?>, IEnumerable<Type?>?> HaveOperator(
		this IThat<IEnumerable<Type?>?> subject,
		Operator @operator,
		Type operand)
	{
		HaveOperatorExpectation expectation = new(@operator, operand);
		return new(subject.ExpectAll(expectation), subject, expectation.Scope);
	}

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> have the
	///     <paramref name="operator" /> with an overload that takes the operand <typeparamref name="TOperand" />.
	/// </summary>
	/// <remarks>
	///     Operators inherited from base types are considered as well; restrict the check to the operators declared
	///     on the type with <see cref="InheritanceScopeResult{TType, TValue}.Directly" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IEnumerable<Type?>, IEnumerable<Type?>?> HaveOperator<TOperand>(
		this IThat<IEnumerable<Type?>?> subject,
		Operator @operator)
		=> subject.HaveOperator(@operator, typeof(TOperand));

	/// <summary>
	///     …have the <paramref name="operator" /> (e.g. <see cref="Operator.Addition" /> matches <c>op_Addition</c>).
	/// </summary>
	/// <inheritdoc cref="HaveOperator(IThat{IEnumerable{Type}}, Operator)" path="/remarks" />
	public static InheritanceScopeResult<IEnumerable<Type?>, IEnumerable<Type?>?> HaveOperator(
		this IEnumerableItems<Type?> items,
		Operator @operator)
	{
		HaveOperatorExpectation expectation = new(@operator, null);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Scope);
	}

	/// <summary>
	///     …have the <paramref name="operator" /> with an overload that takes the <paramref name="operand" />.
	/// </summary>
	/// <inheritdoc cref="HaveOperator(IThat{IEnumerable{Type}}, Operator, Type)" path="/remarks" />
	public static InheritanceScopeResult<IEnumerable<Type?>, IEnumerable<Type?>?> HaveOperator(
		this IEnumerableItems<Type?> items,
		Operator @operator,
		Type operand)
	{
		HaveOperatorExpectation expectation = new(@operator, operand);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Scope);
	}

	/// <summary>
	///     …have the <paramref name="operator" /> with an overload that takes the operand
	///     <typeparamref name="TOperand" />.
	/// </summary>
	/// <inheritdoc cref="HaveOperator{TOperand}(IThat{IEnumerable{Type}}, Operator)" path="/remarks" />
	public static InheritanceScopeResult<IEnumerable<Type?>, IEnumerable<Type?>?> HaveOperator<TOperand>(
		this IEnumerableItems<Type?> items,
		Operator @operator)
		=> items.HaveOperator(@operator, typeof(TOperand));

	/// <inheritdoc cref="HaveOperator(IEnumerableItems{Type}, Operator)" />
	public static InheritanceScopeResult<TCollection, TCollection> HaveOperator<TCollection>(
		this IStructEnumerableItems<TCollection, Type?> items,
		Operator @operator)
		where TCollection : struct, IEnumerable<Type?>
	{
		HaveOperatorExpectation expectation = new(@operator, null);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Scope);
	}

	/// <inheritdoc cref="HaveOperator(IEnumerableItems{Type}, Operator, Type)" />
	public static InheritanceScopeResult<TCollection, TCollection> HaveOperator<TCollection>(
		this IStructEnumerableItems<TCollection, Type?> items,
		Operator @operator,
		Type operand)
		where TCollection : struct, IEnumerable<Type?>
	{
		HaveOperatorExpectation expectation = new(@operator, operand);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Scope);
	}

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="HaveOperator(IThat{IEnumerable{Type}}, Operator)" />
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> HaveOperator(
		this IThat<IAsyncEnumerable<Type?>?> subject,
		Operator @operator)
	{
		HaveOperatorExpectation expectation = new(@operator, null);
		return new(subject.ExpectAll(expectation), subject, expectation.Scope);
	}

	/// <inheritdoc cref="HaveOperator(IThat{IEnumerable{Type}}, Operator, Type)" />
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> HaveOperator(
		this IThat<IAsyncEnumerable<Type?>?> subject,
		Operator @operator,
		Type operand)
	{
		HaveOperatorExpectation expectation = new(@operator, operand);
		return new(subject.ExpectAll(expectation), subject, expectation.Scope);
	}

	/// <inheritdoc cref="HaveOperator{TOperand}(IThat{IEnumerable{Type}}, Operator)" />
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> HaveOperator<TOperand>(
		this IThat<IAsyncEnumerable<Type?>?> subject,
		Operator @operator)
		=> subject.HaveOperator(@operator, typeof(TOperand));

	/// <inheritdoc cref="HaveOperator(IEnumerableItems{Type}, Operator)" />
	public static InheritanceScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> HaveOperator(
		this IAsyncEnumerableItems<Type?> items,
		Operator @operator)
	{
		HaveOperatorExpectation expectation = new(@operator, null);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Scope);
	}

	/// <inheritdoc cref="HaveOperator(IEnumerableItems{Type}, Operator, Type)" />
	public static InheritanceScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> HaveOperator(
		this IAsyncEnumerableItems<Type?> items,
		Operator @operator,
		Type operand)
	{
		HaveOperatorExpectation expectation = new(@operator, operand);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Scope);
	}

	/// <inheritdoc cref="HaveOperator{TOperand}(IEnumerableItems{Type}, Operator)" />
	public static InheritanceScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> HaveOperator<TOperand>(
		this IAsyncEnumerableItems<Type?> items,
		Operator @operator)
		=> items.HaveOperator(@operator, typeof(TOperand));
#endif

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="Type" /> have the
	///     <paramref name="operator" /> (e.g. <see cref="Operator.Addition" /> matches <c>op_Addition</c>).
	/// </summary>
	/// <remarks>
	///     Operators inherited from base types are considered as well; restrict the check to the operators declared
	///     on the type with <see cref="InheritanceScopeResult{TType, TValue}.Directly" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IEnumerable<Type?>, IEnumerable<Type?>?> DoNotHaveOperator(
		this IThat<IEnumerable<Type?>?> subject,
		Operator @operator)
	{
		HaveOperatorExpectation expectation = new(@operator, null);
		return new(subject.ExpectNone(expectation), subject, expectation.Scope);
	}

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="Type" /> have the
	///     <paramref name="operator" /> with an overload that takes the <paramref name="operand" />.
	/// </summary>
	/// <remarks>
	///     Operators inherited from base types are considered as well; restrict the check to the operators declared
	///     on the type with <see cref="InheritanceScopeResult{TType, TValue}.Directly" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IEnumerable<Type?>, IEnumerable<Type?>?> DoNotHaveOperator(
		this IThat<IEnumerable<Type?>?> subject,
		Operator @operator,
		Type operand)
	{
		HaveOperatorExpectation expectation = new(@operator, operand);
		return new(subject.ExpectNone(expectation), subject, expectation.Scope);
	}

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="Type" /> have the
	///     <paramref name="operator" /> with an overload that takes the operand <typeparamref name="TOperand" />.
	/// </summary>
	/// <remarks>
	///     Operators inherited from base types are considered as well; restrict the check to the operators declared
	///     on the type with <see cref="InheritanceScopeResult{TType, TValue}.Directly" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IEnumerable<Type?>, IEnumerable<Type?>?> DoNotHaveOperator<TOperand>(
		this IThat<IEnumerable<Type?>?> subject,
		Operator @operator)
		=> subject.DoNotHaveOperator(@operator, typeof(TOperand));

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="DoNotHaveOperator(IThat{IEnumerable{Type}}, Operator)" />
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> DoNotHaveOperator(
		this IThat<IAsyncEnumerable<Type?>?> subject,
		Operator @operator)
	{
		HaveOperatorExpectation expectation = new(@operator, null);
		return new(subject.ExpectNone(expectation), subject, expectation.Scope);
	}

	/// <inheritdoc cref="DoNotHaveOperator(IThat{IEnumerable{Type}}, Operator, Type)" />
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> DoNotHaveOperator(
		this IThat<IAsyncEnumerable<Type?>?> subject,
		Operator @operator,
		Type operand)
	{
		HaveOperatorExpectation expectation = new(@operator, operand);
		return new(subject.ExpectNone(expectation), subject, expectation.Scope);
	}

	/// <inheritdoc cref="DoNotHaveOperator{TOperand}(IThat{IEnumerable{Type}}, Operator)" />
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> DoNotHaveOperator<TOperand>(
		this IThat<IAsyncEnumerable<Type?>?> subject,
		Operator @operator)
		=> subject.DoNotHaveOperator(@operator, typeof(TOperand));
#endif

	private sealed class HaveOperatorExpectation(Operator @operator, Type? operand) : ItemExpectation<Type>
	{
		private readonly string _operatorText = OperatorText(@operator, operand);

		public OptionsHolder<MemberScopes> Scope { get; } = new(MemberScopes.None);

		public override ValueTask<PredicateResult> IsMatch(Type item, CancellationToken cancellationToken)
			=> new(operand is null
				? item.HasOperator(@operator, !Scope.Value.IsDirect())
				: item.HasOperator(@operator, operand, !Scope.Value.IsDirect()));

		public override void AppendExpectation(StringBuilder stringBuilder, ExpectationGrammars grammars)
		{
			if (Scope.Value.IsDirect())
			{
				stringBuilder.Append(grammars.IsPlural() ? "declare" : "declares");
			}
			else
			{
				stringBuilder.Append(grammars.IsPlural() ? "have" : "has");
			}

			stringBuilder.Append(" the operator ").Append(_operatorText);
		}
	}

	private static string OperatorText(Operator @operator, Type? operand)
	{
		string name = OperatorNames.Display(@operator);
		return operand is null ? name : $"{name} with operand {Formatter.Format(operand)}";
	}
}
