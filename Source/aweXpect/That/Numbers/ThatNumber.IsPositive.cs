using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Results;
#if !NET8_0_OR_GREATER
using System;
#endif

namespace aweXpect;

public static partial class ThatNumber
{
	private const string ExpectIsPositive = "is positive";
	private const string ExpectIsNotPositive = "is not positive";

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the subject is positive.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<TNumber, IThat<TNumber>> IsPositive<TNumber>(
		this IThat<TNumber> source)
		where TNumber : struct, INumber<TNumber>
		=> new(source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsPositiveConstraint<TNumber>(it, grammars)),
			source);

	/// <summary>
	///     Verifies that the subject is positive.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<TNumber?, IThat<TNumber?>> IsPositive<TNumber>(
		this IThat<TNumber?> source)
		where TNumber : struct, INumber<TNumber>
		=> new(source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsPositiveConstraint<TNumber>(it, grammars)),
			source);

	private sealed class IsPositiveConstraint<TNumber>(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithValue<TNumber>(grammars),
			IValueConstraint<TNumber>
		where TNumber : struct, INumber<TNumber>
	{
		public ConstraintResult IsMetBy(TNumber actual)
		{
			Actual = actual;
			Outcome = actual > default(TNumber)
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(ExpectIsPositive);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(ExpectIsNotPositive);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}

	private sealed class NullableIsPositiveConstraint<TNumber>(
		string it,
		ExpectationGrammars grammars)
		: ConstraintResult.WithValue<TNumber?>(grammars),
			IValueConstraint<TNumber?>
		where TNumber : struct, INumber<TNumber>
	{
		public ConstraintResult IsMetBy(TNumber? actual)
		{
			Actual = actual;
			Outcome = actual is not null && actual > default(TNumber)
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(ExpectIsPositive);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(ExpectIsNotPositive);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
#else
	/// <summary>
	///     Verifies that the subject is positive.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<sbyte, IThat<sbyte>> IsPositive(
		this IThat<sbyte> source)
		=> new(source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsPositiveConstraint<sbyte>(it, grammars, a => a > 0)),
			source);

	/// <summary>
	///     Verifies that the subject is positive.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<short, IThat<short>> IsPositive(
		this IThat<short> source)
		=> new(source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsPositiveConstraint<short>(it, grammars, a => a > 0)),
			source);

	/// <summary>
	///     Verifies that the subject is positive.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<int, IThat<int>> IsPositive(
		this IThat<int> source)
		=> new(source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsPositiveConstraint<int>(it, grammars, a => a > 0)),
			source);

	/// <summary>
	///     Verifies that the subject is positive.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<long, IThat<long>> IsPositive(
		this IThat<long> source)
		=> new(source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsPositiveConstraint<long>(it, grammars, a => a > 0L)),
			source);

	/// <summary>
	///     Verifies that the subject is positive.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<float, IThat<float>> IsPositive(
		this IThat<float> source)
		=> new(source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsPositiveConstraint<float>(it, grammars, a => a > 0.0F)),
			source);

	/// <summary>
	///     Verifies that the subject is positive.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<double, IThat<double>> IsPositive(
		this IThat<double> source)
		=> new(source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsPositiveConstraint<double>(it, grammars, a => a > 0.0)),
			source);

	/// <summary>
	///     Verifies that the subject is positive.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<decimal, IThat<decimal>> IsPositive(
		this IThat<decimal> source)
		=> new(source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsPositiveConstraint<decimal>(it, grammars, a => a > 0)),
			source);

	/// <summary>
	///     Verifies that the subject is positive.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<sbyte?, IThat<sbyte?>> IsPositive(
		this IThat<sbyte?> source)
		=> new(source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsPositiveConstraint<sbyte>(it, grammars, a => a > 0)),
			source);

	/// <summary>
	///     Verifies that the subject is positive.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<short?, IThat<short?>> IsPositive(
		this IThat<short?> source)
		=> new(source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsPositiveConstraint<short>(it, grammars, a => a > 0)),
			source);

	/// <summary>
	///     Verifies that the subject is positive.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<int?, IThat<int?>> IsPositive(
		this IThat<int?> source)
		=> new(source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsPositiveConstraint<int>(it, grammars, a => a > 0)),
			source);

	/// <summary>
	///     Verifies that the subject is positive.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<long?, IThat<long?>> IsPositive(
		this IThat<long?> source)
		=> new(source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsPositiveConstraint<long>(it, grammars, a => a > 0L)),
			source);

	/// <summary>
	///     Verifies that the subject is positive.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<float?, IThat<float?>> IsPositive(
		this IThat<float?> source)
		=> new(source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsPositiveConstraint<float>(it, grammars, a => a > 0.0F)),
			source);

	/// <summary>
	///     Verifies that the subject is positive.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<double?, IThat<double?>> IsPositive(
		this IThat<double?> source)
		=> new(source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsPositiveConstraint<double>(it, grammars, a => a > 0.0)),
			source);

	/// <summary>
	///     Verifies that the subject is positive.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<decimal?, IThat<decimal?>> IsPositive(
		this IThat<decimal?> source)
		=> new(source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsPositiveConstraint<decimal>(it, grammars, a => a > 0)),
			source);

	private sealed class IsPositiveConstraint<TNumber>(
		string it,
		ExpectationGrammars grammars,
		Func<TNumber, bool> predicate)
		: ConstraintResult.WithValue<TNumber>(grammars),
			IValueConstraint<TNumber>
		where TNumber : struct, IComparable<TNumber>
	{
		public ConstraintResult IsMetBy(TNumber actual)
		{
			Actual = actual;
			Outcome = predicate(actual) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(ExpectIsPositive);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(ExpectIsNotPositive);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}

	private sealed class NullableIsPositiveConstraint<TNumber>(
		string it,
		ExpectationGrammars grammars,
		Func<TNumber, bool> predicate)
		: ConstraintResult.WithValue<TNumber?>(grammars),
			IValueConstraint<TNumber?>
		where TNumber : struct, IComparable<TNumber>
	{
		public ConstraintResult IsMetBy(TNumber? actual)
		{
			Actual = actual;
			Outcome = actual is not null && predicate(actual.Value) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(ExpectIsPositive);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(ExpectIsNotPositive);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
#endif
}
