using System;
using System.Collections.Generic;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Results;
#if NET8_0_OR_GREATER
using System.Threading;
using System.Threading.Tasks;
#endif

// ReSharper disable PossibleMultipleEnumeration

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
		OptionsHolder<MemberScopes> scope = new(MemberScopes.None);
		return new(subject.Get().ExpectationBuilder.AddConstraint<(Operator Operator, OptionsHolder<MemberScopes> Scope), IEnumerable<Type?>>((@operator, scope),
				static (s, it, grammars)
				=> new HaveOperatorConstraint(it, grammars, s.Operator, null, s.Scope)),
			subject,
			scope);
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
		OptionsHolder<MemberScopes> scope = new(MemberScopes.None);
		return new(subject.Get().ExpectationBuilder.AddConstraint<(Operator Operator, Type Operand, OptionsHolder<MemberScopes> Scope), IEnumerable<Type?>>((@operator, operand, scope),
				static (s, it, grammars)
				=> new HaveOperatorConstraint(it, grammars, s.Operator, s.Operand, s.Scope)),
			subject,
			scope);
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

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> have the
	///     <paramref name="operator" /> (e.g. <see cref="Operator.Addition" /> matches <c>op_Addition</c>).
	/// </summary>
	/// <remarks>
	///     Operators inherited from base types are considered as well; restrict the check to the operators declared
	///     on the type with <see cref="InheritanceScopeResult{TType, TValue}.Directly" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> HaveOperator(
		this IThat<IAsyncEnumerable<Type?>?> subject,
		Operator @operator)
	{
		OptionsHolder<MemberScopes> scope = new(MemberScopes.None);
		return new(subject.Get().ExpectationBuilder.AddConstraint<(Operator Operator, OptionsHolder<MemberScopes> Scope), IAsyncEnumerable<Type?>>((@operator, scope),
				static (s, it, grammars)
				=> new HaveOperatorConstraint(it, grammars, s.Operator, null, s.Scope)),
			subject,
			scope);
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
	public static InheritanceScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> HaveOperator(
		this IThat<IAsyncEnumerable<Type?>?> subject,
		Operator @operator,
		Type operand)
	{
		OptionsHolder<MemberScopes> scope = new(MemberScopes.None);
		return new(subject.Get().ExpectationBuilder.AddConstraint<(Operator Operator, Type Operand, OptionsHolder<MemberScopes> Scope), IAsyncEnumerable<Type?>>((@operator, operand, scope),
				static (s, it, grammars)
				=> new HaveOperatorConstraint(it, grammars, s.Operator, s.Operand, s.Scope)),
			subject,
			scope);
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
	public static InheritanceScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> HaveOperator<TOperand>(
		this IThat<IAsyncEnumerable<Type?>?> subject,
		Operator @operator)
		=> subject.HaveOperator(@operator, typeof(TOperand));
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
		OptionsHolder<MemberScopes> scope = new(MemberScopes.None);
		return new(subject.Get().ExpectationBuilder.AddConstraint<(Operator Operator, OptionsHolder<MemberScopes> Scope), IEnumerable<Type?>>((@operator, scope),
				static (s, it, grammars)
				=> new DoNotHaveOperatorConstraint(it, grammars, s.Operator, null, s.Scope)),
			subject,
			scope);
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
		OptionsHolder<MemberScopes> scope = new(MemberScopes.None);
		return new(subject.Get().ExpectationBuilder.AddConstraint<(Operator Operator, Type Operand, OptionsHolder<MemberScopes> Scope), IEnumerable<Type?>>((@operator, operand, scope),
				static (s, it, grammars)
				=> new DoNotHaveOperatorConstraint(it, grammars, s.Operator, s.Operand, s.Scope)),
			subject,
			scope);
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
	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="Type" /> have the
	///     <paramref name="operator" /> (e.g. <see cref="Operator.Addition" /> matches <c>op_Addition</c>).
	/// </summary>
	/// <remarks>
	///     Operators inherited from base types are considered as well; restrict the check to the operators declared
	///     on the type with <see cref="InheritanceScopeResult{TType, TValue}.Directly" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> DoNotHaveOperator(
		this IThat<IAsyncEnumerable<Type?>?> subject,
		Operator @operator)
	{
		OptionsHolder<MemberScopes> scope = new(MemberScopes.None);
		return new(subject.Get().ExpectationBuilder.AddConstraint<(Operator Operator, OptionsHolder<MemberScopes> Scope), IAsyncEnumerable<Type?>>((@operator, scope),
				static (s, it, grammars)
				=> new DoNotHaveOperatorConstraint(it, grammars, s.Operator, null, s.Scope)),
			subject,
			scope);
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
	public static InheritanceScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> DoNotHaveOperator(
		this IThat<IAsyncEnumerable<Type?>?> subject,
		Operator @operator,
		Type operand)
	{
		OptionsHolder<MemberScopes> scope = new(MemberScopes.None);
		return new(subject.Get().ExpectationBuilder.AddConstraint<(Operator Operator, Type Operand, OptionsHolder<MemberScopes> Scope), IAsyncEnumerable<Type?>>((@operator, operand, scope),
				static (s, it, grammars)
				=> new DoNotHaveOperatorConstraint(it, grammars, s.Operator, s.Operand, s.Scope)),
			subject,
			scope);
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
	public static InheritanceScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> DoNotHaveOperator<TOperand>(
		this IThat<IAsyncEnumerable<Type?>?> subject,
		Operator @operator)
		=> subject.DoNotHaveOperator(@operator, typeof(TOperand));
#endif

	private sealed class HaveOperatorConstraint(
		string it,
		ExpectationGrammars grammars,
		Operator @operator,
		Type? operand,
		OptionsHolder<MemberScopes> scope)
		: CollectionConstraintResult<Type?>(it, grammars),
			IContextConstraint<IEnumerable<Type?>>
#if NET8_0_OR_GREATER
			, IAsyncContextConstraint<IAsyncEnumerable<Type?>>
#endif
	{
		private readonly string _operatorText = OperatorText(@operator, operand);

#if NET8_0_OR_GREATER
		public async ValueTask<ConstraintResult> IsMetBy(IAsyncEnumerable<Type?> actual, IEvaluationContext context,
			CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, context, Matches, cancellationToken);
#endif

		public ConstraintResult IsMetBy(IEnumerable<Type?> actual, IEvaluationContext context)
			=> SetValue(actual, context, Matches);

		private bool Matches(Type? type)
			=> operand is null
				? type.HasOperator(@operator, !scope.Value.IsDirect())
				: type.HasOperator(@operator, operand, !scope.Value.IsDirect());

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(scope.Value.IsDirect() ? "all declare the operator " : "all have the operator ")
				.Append(_operatorText);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It)
				.Append(scope.Value.IsDirect()
					? " contained types not declaring the operator "
					: " contained types without the operator ")
				.Append(_operatorText);

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(scope.Value.IsDirect() ? "not all declare the operator " : "not all have the operator ")
				.Append(_operatorText);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It)
				.Append(scope.Value.IsDirect()
					? " only contained types declaring the operator "
					: " only contained types with the operator ")
				.Append(_operatorText);
	}

	private sealed class DoNotHaveOperatorConstraint(
		string it,
		ExpectationGrammars grammars,
		Operator @operator,
		Type? operand,
		OptionsHolder<MemberScopes> scope)
		: CollectionConstraintResult<Type?>(it, grammars),
			IContextConstraint<IEnumerable<Type?>>
#if NET8_0_OR_GREATER
			, IAsyncContextConstraint<IAsyncEnumerable<Type?>>
#endif
	{
		private readonly string _operatorText = OperatorText(@operator, operand);

#if NET8_0_OR_GREATER
		public async ValueTask<ConstraintResult> IsMetBy(IAsyncEnumerable<Type?> actual, IEvaluationContext context,
			CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, context, type => !Matches(type), cancellationToken);
#endif

		public ConstraintResult IsMetBy(IEnumerable<Type?> actual, IEvaluationContext context)
			=> SetValue(actual, context, type => !Matches(type));

		private bool Matches(Type? type)
			=> operand is null
				? type.HasOperator(@operator, !scope.Value.IsDirect())
				: type.HasOperator(@operator, operand, !scope.Value.IsDirect());

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(scope.Value.IsDirect() ? "all do not declare the operator " : "all do not have the operator ")
				.Append(_operatorText);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It)
				.Append(scope.Value.IsDirect()
					? " contained types declaring the operator "
					: " contained types with the operator ")
				.Append(_operatorText);

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(scope.Value.IsDirect()
					? "also contain a type declaring the operator "
					: "also contain a type with the operator ")
				.Append(_operatorText);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It)
				.Append(scope.Value.IsDirect()
					? " only contained types not declaring the operator "
					: " only contained types without the operator ")
				.Append(_operatorText);
	}

	private static string OperatorText(Operator @operator, Type? operand)
	{
		string name = OperatorNames.Display(@operator);
		return operand is null ? name : $"{name} with operand {Formatter.Format(operand)}";
	}
}
