using System;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Reflection.Helpers;
using aweXpect.Results;

namespace aweXpect.Reflection;

public static partial class ThatType
{
	/// <summary>
	///     Verifies that the <see cref="Type" /> declares the <paramref name="operator" />
	///     (e.g. <see cref="Operator.Addition" /> matches <c>op_Addition</c>).
	/// </summary>
	/// <param name="subject">The type subject.</param>
	/// <param name="operator">The <see cref="Operator" /> to look for.</param>
	/// <param name="inherit">
	///     <see langword="true" /> to also consider operators inherited from base types; otherwise,
	///     <see langword="false" /> (the default).
	/// </param>
	public static AndOrResult<Type?, IThat<Type?>> HasOperator(
		this IThat<Type?> subject,
		Operator @operator,
		bool inherit = false)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new HasOperatorConstraint(it, grammars, @operator, null, inherit)),
			subject);

	/// <summary>
	///     Verifies that the <see cref="Type" /> declares the <paramref name="operator" /> with an overload that takes
	///     the operand <typeparamref name="TOperand" /> as one of its parameters.
	/// </summary>
	/// <typeparam name="TOperand">The operand type that one of the operator's parameters must match exactly.</typeparam>
	/// <param name="subject">The type subject.</param>
	/// <param name="operator">The <see cref="Operator" /> to look for.</param>
	/// <param name="inherit">
	///     <see langword="true" /> to also consider operators inherited from base types; otherwise,
	///     <see langword="false" /> (the default).
	/// </param>
	public static AndOrResult<Type?, IThat<Type?>> HasOperator<TOperand>(
		this IThat<Type?> subject,
		Operator @operator,
		bool inherit = false)
		=> subject.HasOperator(@operator, typeof(TOperand), inherit);

	/// <summary>
	///     Verifies that the <see cref="Type" /> declares the <paramref name="operator" /> with an overload that takes
	///     the <paramref name="operand" /> as one of its parameters.
	/// </summary>
	/// <param name="subject">The type subject.</param>
	/// <param name="operator">The <see cref="Operator" /> to look for.</param>
	/// <param name="operand">The operand type that one of the operator's parameters must match exactly.</param>
	/// <param name="inherit">
	///     <see langword="true" /> to also consider operators inherited from base types; otherwise,
	///     <see langword="false" /> (the default).
	/// </param>
	public static AndOrResult<Type?, IThat<Type?>> HasOperator(
		this IThat<Type?> subject,
		Operator @operator,
		Type operand,
		bool inherit = false)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new HasOperatorConstraint(it, grammars, @operator, operand, inherit)),
			subject);

	/// <summary>
	///     Verifies that the <see cref="Type" /> does not declare the <paramref name="operator" />
	///     (e.g. <see cref="Operator.Addition" /> matches <c>op_Addition</c>).
	/// </summary>
	/// <param name="subject">The type subject.</param>
	/// <param name="operator">The <see cref="Operator" /> to look for.</param>
	/// <param name="inherit">
	///     <see langword="true" /> to also consider operators inherited from base types; otherwise,
	///     <see langword="false" /> (the default).
	/// </param>
	public static AndOrResult<Type?, IThat<Type?>> DoesNotHaveOperator(
		this IThat<Type?> subject,
		Operator @operator,
		bool inherit = false)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new HasOperatorConstraint(it, grammars, @operator, null, inherit).Invert()),
			subject);

	/// <summary>
	///     Verifies that the <see cref="Type" /> does not declare the <paramref name="operator" /> with an overload that
	///     takes the operand <typeparamref name="TOperand" /> as one of its parameters.
	/// </summary>
	/// <typeparam name="TOperand">The operand type that one of the operator's parameters must match exactly.</typeparam>
	/// <param name="subject">The type subject.</param>
	/// <param name="operator">The <see cref="Operator" /> to look for.</param>
	/// <param name="inherit">
	///     <see langword="true" /> to also consider operators inherited from base types; otherwise,
	///     <see langword="false" /> (the default).
	/// </param>
	public static AndOrResult<Type?, IThat<Type?>> DoesNotHaveOperator<TOperand>(
		this IThat<Type?> subject,
		Operator @operator,
		bool inherit = false)
		=> subject.DoesNotHaveOperator(@operator, typeof(TOperand), inherit);

	/// <summary>
	///     Verifies that the <see cref="Type" /> does not declare the <paramref name="operator" /> with an overload that
	///     takes the <paramref name="operand" /> as one of its parameters.
	/// </summary>
	/// <param name="subject">The type subject.</param>
	/// <param name="operator">The <see cref="Operator" /> to look for.</param>
	/// <param name="operand">The operand type that one of the operator's parameters must match exactly.</param>
	/// <param name="inherit">
	///     <see langword="true" /> to also consider operators inherited from base types; otherwise,
	///     <see langword="false" /> (the default).
	/// </param>
	public static AndOrResult<Type?, IThat<Type?>> DoesNotHaveOperator(
		this IThat<Type?> subject,
		Operator @operator,
		Type operand,
		bool inherit = false)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new HasOperatorConstraint(it, grammars, @operator, operand, inherit).Invert()),
			subject);

	private sealed class HasOperatorConstraint(
		string it,
		ExpectationGrammars grammars,
		Operator @operator,
		Type? operand,
		bool inherit)
		: ConstraintResult.WithNotNullValue<Type?>(it, grammars),
			IValueConstraint<Type?>
	{
		private readonly string _name = OperatorNames.Display(@operator);

		public ConstraintResult IsMetBy(Type? actual)
		{
			Actual = actual;
			bool hasOperator = operand is null
				? actual.HasOperator(@operator, inherit)
				: actual.HasOperator(@operator, operand, inherit);
			Outcome = hasOperator ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendOperatorExpectation(stringBuilder, "has the operator ");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" did not have the operator ").Append(_name);
			AppendOperand(stringBuilder);
			stringBuilder.Append(' ');
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendOperatorExpectation(stringBuilder, "does not have the operator ");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" had the operator ").Append(_name);
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
