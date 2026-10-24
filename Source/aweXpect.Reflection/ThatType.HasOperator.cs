using System;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Reflection;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Results;

namespace aweXpect;

public static partial class ThatType
{
	/// <summary>
	///     Verifies that the <see cref="Type" /> has the <paramref name="operator" />
	///     (e.g. <see cref="Operator.Addition" /> matches <c>op_Addition</c>).
	/// </summary>
	/// <remarks>
	///     Operators inherited from base types are considered as well; restrict the check to the operators declared
	///     on the type with <see cref="InheritanceScopeResult{TType, TValue}.Directly" />.
	/// </remarks>
	/// <param name="subject">The type subject.</param>
	/// <param name="operator">The <see cref="Operator" /> to look for.</param>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<Type, Type?> HasOperator(
		this IThat<Type?> subject,
		Operator @operator)
	{
		ScopeOptions scope = new();
		return new(subject.Get().ExpectationBuilder.AddConstraint((Operator: @operator, Scope: scope),
				static (s, it, grammars)
				=> new HasOperatorConstraint(it, grammars, s.Operator, null, s.Scope)),
			subject,
			scope);
	}

	/// <summary>
	///     Verifies that the <see cref="Type" /> has the <paramref name="operator" /> with an overload that takes
	///     the operand <typeparamref name="TOperand" /> as one of its parameters.
	/// </summary>
	/// <remarks>
	///     Operators inherited from base types are considered as well; restrict the check to the operators declared
	///     on the type with <see cref="InheritanceScopeResult{TType, TValue}.Directly" />.
	/// </remarks>
	/// <typeparam name="TOperand">The operand type that one of the operator's parameters must match exactly.</typeparam>
	/// <param name="subject">The type subject.</param>
	/// <param name="operator">The <see cref="Operator" /> to look for.</param>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<Type, Type?> HasOperator<TOperand>(
		this IThat<Type?> subject,
		Operator @operator)
		=> subject.HasOperator(@operator, typeof(TOperand));

	/// <summary>
	///     Verifies that the <see cref="Type" /> has the <paramref name="operator" /> with an overload that takes
	///     the <paramref name="operand" /> as one of its parameters.
	/// </summary>
	/// <remarks>
	///     Operators inherited from base types are considered as well; restrict the check to the operators declared
	///     on the type with <see cref="InheritanceScopeResult{TType, TValue}.Directly" />.
	/// </remarks>
	/// <param name="subject">The type subject.</param>
	/// <param name="operator">The <see cref="Operator" /> to look for.</param>
	/// <param name="operand">The operand type that one of the operator's parameters must match exactly.</param>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<Type, Type?> HasOperator(
		this IThat<Type?> subject,
		Operator @operator,
		Type operand)
	{
		ScopeOptions scope = new();
		return new(subject.Get().ExpectationBuilder.AddConstraint((Operator: @operator, Operand: operand, Scope: scope),
				static (s, it, grammars)
				=> new HasOperatorConstraint(it, grammars, s.Operator, s.Operand, s.Scope)),
			subject,
			scope);
	}

	/// <summary>
	///     Verifies that the <see cref="Type" /> does not have the <paramref name="operator" />
	///     (e.g. <see cref="Operator.Addition" /> matches <c>op_Addition</c>).
	/// </summary>
	/// <remarks>
	///     Operators inherited from base types are considered as well; restrict the check to the operators declared
	///     on the type with <see cref="InheritanceScopeResult{TType, TValue}.Directly" />.
	/// </remarks>
	/// <param name="subject">The type subject.</param>
	/// <param name="operator">The <see cref="Operator" /> to look for.</param>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<Type, Type?> DoesNotHaveOperator(
		this IThat<Type?> subject,
		Operator @operator)
	{
		ScopeOptions scope = new();
		return new(subject.Get().ExpectationBuilder.AddConstraint((Operator: @operator, Scope: scope),
				static (s, it, grammars)
				=> new HasOperatorConstraint(it, grammars, s.Operator, null, s.Scope).Invert()),
			subject,
			scope);
	}

	/// <summary>
	///     Verifies that the <see cref="Type" /> does not have the <paramref name="operator" /> with an overload that
	///     takes the operand <typeparamref name="TOperand" /> as one of its parameters.
	/// </summary>
	/// <remarks>
	///     Operators inherited from base types are considered as well; restrict the check to the operators declared
	///     on the type with <see cref="InheritanceScopeResult{TType, TValue}.Directly" />.
	/// </remarks>
	/// <typeparam name="TOperand">The operand type that one of the operator's parameters must match exactly.</typeparam>
	/// <param name="subject">The type subject.</param>
	/// <param name="operator">The <see cref="Operator" /> to look for.</param>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<Type, Type?> DoesNotHaveOperator<TOperand>(
		this IThat<Type?> subject,
		Operator @operator)
		=> subject.DoesNotHaveOperator(@operator, typeof(TOperand));

	/// <summary>
	///     Verifies that the <see cref="Type" /> does not have the <paramref name="operator" /> with an overload that
	///     takes the <paramref name="operand" /> as one of its parameters.
	/// </summary>
	/// <remarks>
	///     Operators inherited from base types are considered as well; restrict the check to the operators declared
	///     on the type with <see cref="InheritanceScopeResult{TType, TValue}.Directly" />.
	/// </remarks>
	/// <param name="subject">The type subject.</param>
	/// <param name="operator">The <see cref="Operator" /> to look for.</param>
	/// <param name="operand">The operand type that one of the operator's parameters must match exactly.</param>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<Type, Type?> DoesNotHaveOperator(
		this IThat<Type?> subject,
		Operator @operator,
		Type operand)
	{
		ScopeOptions scope = new();
		return new(subject.Get().ExpectationBuilder.AddConstraint((Operator: @operator, Operand: operand, Scope: scope),
				static (s, it, grammars)
				=> new HasOperatorConstraint(it, grammars, s.Operator, s.Operand, s.Scope).Invert()),
			subject,
			scope);
	}

	private sealed class HasOperatorConstraint(
		string it,
		ExpectationGrammars grammars,
		Operator @operator,
		Type? operand,
		ScopeOptions scope)
		: ConstraintResult.WithNotNullValue<Type?>(it, grammars),
			IValueConstraint<Type?>
	{
		private readonly string _name = OperatorNames.Display(@operator);

		public ConstraintResult IsMetBy(Type? actual)
		{
			Actual = actual;
			bool hasOperator = operand is null
				? actual.HasOperator(@operator, !scope.IsDirect)
				: actual.HasOperator(@operator, operand, !scope.IsDirect);
			Outcome = hasOperator ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendOperatorExpectation(stringBuilder, scope.IsDirect ? "declares the operator " : "has the operator ");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(scope.IsDirect ? " did not declare the operator " : " did not have the operator ")
				.Append(_name);
			AppendOperand(stringBuilder);
			stringBuilder.Append(' ');
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendOperatorExpectation(stringBuilder,
				scope.IsDirect ? "does not declare the operator " : "does not have the operator ");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(scope.IsDirect ? " declared the operator " : " had the operator ").Append(_name);
			AppendOperand(stringBuilder);
			stringBuilder.Append(' ');
			Formatter.Format(stringBuilder, Actual);
		}

		private void AppendOperatorExpectation(StringBuilder stringBuilder, string verb)
		{
			stringBuilder.Append(verb).Append(_name);
			AppendOperand(stringBuilder);
		}

		private void AppendOperand(StringBuilder stringBuilder)
		{
			if (operand is null)
			{
				return;
			}

			stringBuilder.Append(" with operand ");
			Formatter.Format(stringBuilder, operand);
		}
	}
}
