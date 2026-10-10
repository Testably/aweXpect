using System;
using System.Collections.Generic;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;
using aweXpect.Results;
#if NET8_0_OR_GREATER
using System.Threading;
using System.Threading.Tasks;
#endif

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Reflection;

public static partial class ThatTypes
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> declare the
	///     <paramref name="operator" /> (e.g. <see cref="Operator.Addition" /> matches <c>op_Addition</c>).
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>?>> HaveOperator(
		this IThat<IEnumerable<Type?>?> subject,
		Operator @operator,
		bool inherit = false)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<(Operator Operator, bool Inherit), IEnumerable<Type?>>((@operator, inherit),
				static (s, it, grammars)
				=> new HaveOperatorConstraint(it, grammars, s.Operator, null, s.Inherit)),
			subject);

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> declare the
	///     <paramref name="operator" /> with an overload that takes the <paramref name="operand" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>?>> HaveOperator(
		this IThat<IEnumerable<Type?>?> subject,
		Operator @operator,
		Type operand,
		bool inherit = false)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<(Operator Operator, Type Operand, bool Inherit), IEnumerable<Type?>>((@operator, operand, inherit),
				static (s, it, grammars)
				=> new HaveOperatorConstraint(it, grammars, s.Operator, s.Operand, s.Inherit)),
			subject);

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> declare the
	///     <paramref name="operator" /> with an overload that takes the operand <typeparamref name="TOperand" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>?>> HaveOperator<TOperand>(
		this IThat<IEnumerable<Type?>?> subject,
		Operator @operator,
		bool inherit = false)
		=> subject.HaveOperator(@operator, typeof(TOperand), inherit);

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> declare the
	///     <paramref name="operator" /> (e.g. <see cref="Operator.Addition" /> matches <c>op_Addition</c>).
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>?>> HaveOperator(
		this IThat<IAsyncEnumerable<Type?>?> subject,
		Operator @operator,
		bool inherit = false)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<(Operator Operator, bool Inherit), IAsyncEnumerable<Type?>>((@operator, inherit),
				static (s, it, grammars)
				=> new HaveOperatorConstraint(it, grammars, s.Operator, null, s.Inherit)),
			subject);

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> declare the
	///     <paramref name="operator" /> with an overload that takes the <paramref name="operand" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>?>> HaveOperator(
		this IThat<IAsyncEnumerable<Type?>?> subject,
		Operator @operator,
		Type operand,
		bool inherit = false)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<(Operator Operator, Type Operand, bool Inherit), IAsyncEnumerable<Type?>>((@operator, operand, inherit),
				static (s, it, grammars)
				=> new HaveOperatorConstraint(it, grammars, s.Operator, s.Operand, s.Inherit)),
			subject);

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> declare the
	///     <paramref name="operator" /> with an overload that takes the operand <typeparamref name="TOperand" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>?>> HaveOperator<TOperand>(
		this IThat<IAsyncEnumerable<Type?>?> subject,
		Operator @operator,
		bool inherit = false)
		=> subject.HaveOperator(@operator, typeof(TOperand), inherit);
#endif

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="Type" /> declare the
	///     <paramref name="operator" /> (e.g. <see cref="Operator.Addition" /> matches <c>op_Addition</c>).
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>?>> DoNotHaveOperator(
		this IThat<IEnumerable<Type?>?> subject,
		Operator @operator,
		bool inherit = false)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<(Operator Operator, bool Inherit), IEnumerable<Type?>>((@operator, inherit),
				static (s, it, grammars)
				=> new DoNotHaveOperatorConstraint(it, grammars, s.Operator, null, s.Inherit)),
			subject);

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="Type" /> declare the
	///     <paramref name="operator" /> with an overload that takes the <paramref name="operand" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>?>> DoNotHaveOperator(
		this IThat<IEnumerable<Type?>?> subject,
		Operator @operator,
		Type operand,
		bool inherit = false)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<(Operator Operator, Type Operand, bool Inherit), IEnumerable<Type?>>((@operator, operand, inherit),
				static (s, it, grammars)
				=> new DoNotHaveOperatorConstraint(it, grammars, s.Operator, s.Operand, s.Inherit)),
			subject);

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="Type" /> declare the
	///     <paramref name="operator" /> with an overload that takes the operand <typeparamref name="TOperand" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>?>> DoNotHaveOperator<TOperand>(
		this IThat<IEnumerable<Type?>?> subject,
		Operator @operator,
		bool inherit = false)
		=> subject.DoNotHaveOperator(@operator, typeof(TOperand), inherit);

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="Type" /> declare the
	///     <paramref name="operator" /> (e.g. <see cref="Operator.Addition" /> matches <c>op_Addition</c>).
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>?>> DoNotHaveOperator(
		this IThat<IAsyncEnumerable<Type?>?> subject,
		Operator @operator,
		bool inherit = false)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<(Operator Operator, bool Inherit), IAsyncEnumerable<Type?>>((@operator, inherit),
				static (s, it, grammars)
				=> new DoNotHaveOperatorConstraint(it, grammars, s.Operator, null, s.Inherit)),
			subject);

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="Type" /> declare the
	///     <paramref name="operator" /> with an overload that takes the <paramref name="operand" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>?>> DoNotHaveOperator(
		this IThat<IAsyncEnumerable<Type?>?> subject,
		Operator @operator,
		Type operand,
		bool inherit = false)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<(Operator Operator, Type Operand, bool Inherit), IAsyncEnumerable<Type?>>((@operator, operand, inherit),
				static (s, it, grammars)
				=> new DoNotHaveOperatorConstraint(it, grammars, s.Operator, s.Operand, s.Inherit)),
			subject);

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="Type" /> declare the
	///     <paramref name="operator" /> with an overload that takes the operand <typeparamref name="TOperand" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>?>> DoNotHaveOperator<TOperand>(
		this IThat<IAsyncEnumerable<Type?>?> subject,
		Operator @operator,
		bool inherit = false)
		=> subject.DoNotHaveOperator(@operator, typeof(TOperand), inherit);
#endif

	private sealed class HaveOperatorConstraint(
		string it,
		ExpectationGrammars grammars,
		Operator @operator,
		Type? operand,
		bool inherit)
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
				? type.HasOperator(@operator, inherit)
				: type.HasOperator(@operator, operand, inherit);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("all have the operator ").Append(_operatorText);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" contained types without the operator ").Append(_operatorText)
				;
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("not all have the operator ").Append(_operatorText);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" only contained types with the operator ").Append(_operatorText)
				;
		}
	}

	private sealed class DoNotHaveOperatorConstraint(
		string it,
		ExpectationGrammars grammars,
		Operator @operator,
		Type? operand,
		bool inherit)
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
				? type.HasOperator(@operator, inherit)
				: type.HasOperator(@operator, operand, inherit);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("all do not have the operator ").Append(_operatorText);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" contained types with the operator ").Append(_operatorText);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("also contain a type with the operator ").Append(_operatorText);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" only contained types without the operator ").Append(_operatorText)
				;
		}
	}

	private static string OperatorText(Operator @operator, Type? operand)
	{
		string name = OperatorNames.Display(@operator);
		return operand is null ? name : $"{name} with operand {Formatter.Format(operand)}";
	}
}
