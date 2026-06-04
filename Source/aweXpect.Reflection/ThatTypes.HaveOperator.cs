using System;
using System.Collections.Generic;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
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
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>>> HaveOperator(
		this IThat<IEnumerable<Type?>> subject,
		Operator @operator,
		bool inherit = false)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IEnumerable<Type?>>((it, grammars)
				=> new HaveOperatorConstraint(it, grammars, @operator, null, inherit)),
			subject);

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> declare the
	///     <paramref name="operator" /> with an overload that takes the <paramref name="operand" />.
	/// </summary>
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>>> HaveOperator(
		this IThat<IEnumerable<Type?>> subject,
		Operator @operator,
		Type operand,
		bool inherit = false)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IEnumerable<Type?>>((it, grammars)
				=> new HaveOperatorConstraint(it, grammars, @operator, operand, inherit)),
			subject);

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> declare the
	///     <paramref name="operator" /> with an overload that takes the operand <typeparamref name="TOperand" />.
	/// </summary>
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>>> HaveOperator<TOperand>(
		this IThat<IEnumerable<Type?>> subject,
		Operator @operator,
		bool inherit = false)
		=> subject.HaveOperator(@operator, typeof(TOperand), inherit);

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> declare the
	///     <paramref name="operator" /> (e.g. <see cref="Operator.Addition" /> matches <c>op_Addition</c>).
	/// </summary>
	public static AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>>> HaveOperator(
		this IThat<IAsyncEnumerable<Type?>> subject,
		Operator @operator,
		bool inherit = false)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IAsyncEnumerable<Type?>>((it, grammars)
				=> new HaveOperatorConstraint(it, grammars, @operator, null, inherit)),
			subject);

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> declare the
	///     <paramref name="operator" /> with an overload that takes the <paramref name="operand" />.
	/// </summary>
	public static AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>>> HaveOperator(
		this IThat<IAsyncEnumerable<Type?>> subject,
		Operator @operator,
		Type operand,
		bool inherit = false)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IAsyncEnumerable<Type?>>((it, grammars)
				=> new HaveOperatorConstraint(it, grammars, @operator, operand, inherit)),
			subject);

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> declare the
	///     <paramref name="operator" /> with an overload that takes the operand <typeparamref name="TOperand" />.
	/// </summary>
	public static AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>>> HaveOperator<TOperand>(
		this IThat<IAsyncEnumerable<Type?>> subject,
		Operator @operator,
		bool inherit = false)
		=> subject.HaveOperator(@operator, typeof(TOperand), inherit);
#endif

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="Type" /> declare the
	///     <paramref name="operator" /> (e.g. <see cref="Operator.Addition" /> matches <c>op_Addition</c>).
	/// </summary>
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>>> DoNotHaveOperator(
		this IThat<IEnumerable<Type?>> subject,
		Operator @operator,
		bool inherit = false)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IEnumerable<Type?>>((it, grammars)
				=> new DoNotHaveOperatorConstraint(it, grammars, @operator, null, inherit)),
			subject);

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="Type" /> declare the
	///     <paramref name="operator" /> with an overload that takes the <paramref name="operand" />.
	/// </summary>
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>>> DoNotHaveOperator(
		this IThat<IEnumerable<Type?>> subject,
		Operator @operator,
		Type operand,
		bool inherit = false)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IEnumerable<Type?>>((it, grammars)
				=> new DoNotHaveOperatorConstraint(it, grammars, @operator, operand, inherit)),
			subject);

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="Type" /> declare the
	///     <paramref name="operator" /> with an overload that takes the operand <typeparamref name="TOperand" />.
	/// </summary>
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>>> DoNotHaveOperator<TOperand>(
		this IThat<IEnumerable<Type?>> subject,
		Operator @operator,
		bool inherit = false)
		=> subject.DoNotHaveOperator(@operator, typeof(TOperand), inherit);

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="Type" /> declare the
	///     <paramref name="operator" /> (e.g. <see cref="Operator.Addition" /> matches <c>op_Addition</c>).
	/// </summary>
	public static AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>>> DoNotHaveOperator(
		this IThat<IAsyncEnumerable<Type?>> subject,
		Operator @operator,
		bool inherit = false)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IAsyncEnumerable<Type?>>((it, grammars)
				=> new DoNotHaveOperatorConstraint(it, grammars, @operator, null, inherit)),
			subject);

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="Type" /> declare the
	///     <paramref name="operator" /> with an overload that takes the <paramref name="operand" />.
	/// </summary>
	public static AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>>> DoNotHaveOperator(
		this IThat<IAsyncEnumerable<Type?>> subject,
		Operator @operator,
		Type operand,
		bool inherit = false)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IAsyncEnumerable<Type?>>((it, grammars)
				=> new DoNotHaveOperatorConstraint(it, grammars, @operator, operand, inherit)),
			subject);

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="Type" /> declare the
	///     <paramref name="operator" /> with an overload that takes the operand <typeparamref name="TOperand" />.
	/// </summary>
	public static AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>>> DoNotHaveOperator<TOperand>(
		this IThat<IAsyncEnumerable<Type?>> subject,
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
		: CollectionConstraintResult<Type?>(grammars),
			IValueConstraint<IEnumerable<Type?>>
#if NET8_0_OR_GREATER
			, IAsyncConstraint<IAsyncEnumerable<Type?>>
#endif
	{
		private readonly string _operatorText = OperatorText(@operator, operand);

#if NET8_0_OR_GREATER
		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<Type?> actual,
			CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, Matches);
#endif

		public ConstraintResult IsMetBy(IEnumerable<Type?> actual)
			=> SetValue(actual, Matches);

		private bool Matches(Type? type)
			=> operand is null
				? type.HasOperator(@operator, inherit)
				: type.HasOperator(@operator, operand, inherit);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("all have the operator ").Append(_operatorText);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained types without the operator ").Append(_operatorText)
				.Append(' ');
			Formatter.Format(stringBuilder, NotMatching, FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("not all have the operator ").Append(_operatorText);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained types with the operator ").Append(_operatorText)
				.Append(' ');
			Formatter.Format(stringBuilder, Matching, FormattingOptions.Indented(indentation));
		}
	}

	private sealed class DoNotHaveOperatorConstraint(
		string it,
		ExpectationGrammars grammars,
		Operator @operator,
		Type? operand,
		bool inherit)
		: CollectionConstraintResult<Type?>(grammars),
			IValueConstraint<IEnumerable<Type?>>
#if NET8_0_OR_GREATER
			, IAsyncConstraint<IAsyncEnumerable<Type?>>
#endif
	{
		private readonly string _operatorText = OperatorText(@operator, operand);

#if NET8_0_OR_GREATER
		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<Type?> actual,
			CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, type => !Matches(type));
#endif

		public ConstraintResult IsMetBy(IEnumerable<Type?> actual)
			=> SetValue(actual, type => !Matches(type));

		private bool Matches(Type? type)
			=> operand is null
				? type.HasOperator(@operator, inherit)
				: type.HasOperator(@operator, operand, inherit);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("all do not have the operator ").Append(_operatorText);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained types with the operator ").Append(_operatorText).Append(' ');
			Formatter.Format(stringBuilder, NotMatching, FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("also contain a type with the operator ").Append(_operatorText);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained types without the operator ").Append(_operatorText)
				.Append(' ');
			Formatter.Format(stringBuilder, Matching, FormattingOptions.Indented(indentation));
		}
	}

	private static string OperatorText(Operator @operator, Type? operand)
	{
		string name = OperatorNames.Display(@operator);
		return operand is null ? name : $"{name} with operand {Formatter.Format(operand)}";
	}
}
