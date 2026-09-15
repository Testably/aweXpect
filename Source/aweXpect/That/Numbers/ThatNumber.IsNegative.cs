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
	private const string ExpectIsNegative = "is negative";
	private const string ExpectIsNotNegative = "is not negative";

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the subject is negative.
	/// </summary>
	public static AndOrResult<TNumber, IThat<TNumber>> IsNegative<TNumber>(
		this IThat<TNumber> subject)
		where TNumber : struct, INumber<TNumber>
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsNegativeConstraint<TNumber>(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that the subject is negative.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<TNumber?, IThat<TNumber?>> IsNegative<TNumber>(
		this IThat<TNumber?> subject)
		where TNumber : struct, INumber<TNumber>
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsNegativeConstraint<TNumber>(it, grammars)),
			subject);

	private sealed class IsNegativeConstraint<TNumber>(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithValue<TNumber>(grammars),
			IValueConstraint<TNumber>
		where TNumber : struct, INumber<TNumber>
	{
		public ConstraintResult IsMetBy(TNumber actual)
		{
			Actual = actual;
			Outcome = actual < default(TNumber)
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(ExpectIsNegative);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(ExpectIsNotNegative);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}

	private sealed class NullableIsNegativeConstraint<TNumber>(
		string it,
		ExpectationGrammars grammars)
		: ConstraintResult.WithNotNullValue<TNumber?>(it, grammars),
			IValueConstraint<TNumber?>
		where TNumber : struct, INumber<TNumber>
	{
		public ConstraintResult IsMetBy(TNumber? actual)
		{
			Actual = actual;
			Outcome = actual is not null && actual < default(TNumber)
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(ExpectIsNegative);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(ExpectIsNotNegative);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
#else
	/// <summary>
	///     Verifies that the subject is negative.
	/// </summary>
	public static AndOrResult<sbyte, IThat<sbyte>> IsNegative(
		this IThat<sbyte> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsNegativeConstraint<sbyte>(it, grammars, a => a < 0)),
			subject);

	/// <summary>
	///     Verifies that the subject is negative.
	/// </summary>
	public static AndOrResult<short, IThat<short>> IsNegative(
		this IThat<short> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsNegativeConstraint<short>(it, grammars, a => a < 0)),
			subject);

	/// <summary>
	///     Verifies that the subject is negative.
	/// </summary>
	public static AndOrResult<int, IThat<int>> IsNegative(
		this IThat<int> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsNegativeConstraint<int>(it, grammars, a => a < 0)),
			subject);

	/// <summary>
	///     Verifies that the subject is negative.
	/// </summary>
	public static AndOrResult<long, IThat<long>> IsNegative(
		this IThat<long> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsNegativeConstraint<long>(it, grammars, a => a < 0L)),
			subject);

	/// <summary>
	///     Verifies that the subject is negative.
	/// </summary>
	public static AndOrResult<float, IThat<float>> IsNegative(
		this IThat<float> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsNegativeConstraint<float>(it, grammars, a => a < 0.0F)),
			subject);

	/// <summary>
	///     Verifies that the subject is negative.
	/// </summary>
	public static AndOrResult<double, IThat<double>> IsNegative(
		this IThat<double> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsNegativeConstraint<double>(it, grammars, a => a < 0.0)),
			subject);

	/// <summary>
	///     Verifies that the subject is negative.
	/// </summary>
	public static AndOrResult<decimal, IThat<decimal>> IsNegative(
		this IThat<decimal> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsNegativeConstraint<decimal>(it, grammars, a => a < 0)),
			subject);

	/// <summary>
	///     Verifies that the subject is negative.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<sbyte?, IThat<sbyte?>> IsNegative(
		this IThat<sbyte?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsNegativeConstraint<sbyte>(it, grammars, a => a < 0)),
			subject);

	/// <summary>
	///     Verifies that the subject is negative.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<short?, IThat<short?>> IsNegative(
		this IThat<short?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsNegativeConstraint<short>(it, grammars, a => a < 0)),
			subject);

	/// <summary>
	///     Verifies that the subject is negative.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<int?, IThat<int?>> IsNegative(
		this IThat<int?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsNegativeConstraint<int>(it, grammars, a => a < 0)),
			subject);

	/// <summary>
	///     Verifies that the subject is negative.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<long?, IThat<long?>> IsNegative(
		this IThat<long?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsNegativeConstraint<long>(it, grammars, a => a < 0L)),
			subject);

	/// <summary>
	///     Verifies that the subject is negative.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<float?, IThat<float?>> IsNegative(
		this IThat<float?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsNegativeConstraint<float>(it, grammars, a => a < 0.0F)),
			subject);

	/// <summary>
	///     Verifies that the subject is negative.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<double?, IThat<double?>> IsNegative(
		this IThat<double?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsNegativeConstraint<double>(it, grammars, a => a < 0.0)),
			subject);

	/// <summary>
	///     Verifies that the subject is negative.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<decimal?, IThat<decimal?>> IsNegative(
		this IThat<decimal?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsNegativeConstraint<decimal>(it, grammars, a => a < 0)),
			subject);

	private sealed class IsNegativeConstraint<TNumber>(
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
			=> stringBuilder.Append(ExpectIsNegative);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(ExpectIsNotNegative);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}

	private sealed class NullableIsNegativeConstraint<TNumber>(
		string it,
		ExpectationGrammars grammars,
		Func<TNumber, bool> predicate)
		: ConstraintResult.WithNotNullValue<TNumber?>(it, grammars),
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
			=> stringBuilder.Append(ExpectIsNegative);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(ExpectIsNotNegative);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
#endif
}
