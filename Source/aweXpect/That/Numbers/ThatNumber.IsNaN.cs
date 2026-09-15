using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatNumber
{
	private const string ExpectIsNaN = "is NaN";
	private const string ExpectIsNotNaN = "is not NaN";

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the subject is seen as not a number.
	/// </summary>
	public static AndOrResult<TNumber, IThat<TNumber>> IsNaN<TNumber>(this IThat<TNumber> subject)
		where TNumber : struct, IFloatingPoint<TNumber>
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsNaNConstraint<TNumber>(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that the subject is seen as not a number.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<TNumber?, IThat<TNumber?>> IsNaN<TNumber>(this IThat<TNumber?> subject)
		where TNumber : struct, IFloatingPoint<TNumber>
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsNaNConstraint<TNumber>(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that the subject is not seen as not a number.
	/// </summary>
	public static AndOrResult<TNumber, IThat<TNumber>> IsNotNaN<TNumber>(this IThat<TNumber> subject)
		where TNumber : struct, IFloatingPoint<TNumber>
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsNaNConstraint<TNumber>(it, grammars).Invert()),
			subject);

	/// <summary>
	///     Verifies that the subject is not seen as not a number.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<TNumber?, IThat<TNumber?>> IsNotNaN<TNumber>(this IThat<TNumber?> subject)
		where TNumber : struct, IFloatingPoint<TNumber>
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsNaNConstraint<TNumber>(it, grammars).Invert()),
			subject);

	private sealed class IsNaNConstraint<TNumber>(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithValue<TNumber>(grammars),
			IValueConstraint<TNumber>
		where TNumber : struct, IFloatingPoint<TNumber>
	{
		public ConstraintResult IsMetBy(TNumber actual)
		{
			Actual = actual;
			Outcome = TNumber.IsNaN(actual) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(ExpectIsNaN);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(ExpectIsNotNaN);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}

	private sealed class NullableIsNaNConstraint<TNumber>(
		string it,
		ExpectationGrammars grammars)
		: ConstraintResult.WithNotNullValue<TNumber?>(it, grammars),
			IValueConstraint<TNumber?>
		where TNumber : struct, IFloatingPoint<TNumber>
	{
		public ConstraintResult IsMetBy(TNumber? actual)
		{
			Actual = actual;
			Outcome = actual is not null && TNumber.IsNaN(actual.Value) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(ExpectIsNaN);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(ExpectIsNotNaN);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
#else
	/// <summary>
	///     Verifies that the subject is seen as not a number (<see cref="float.NaN" />).
	/// </summary>
	public static AndOrResult<float, IThat<float>> IsNaN(this IThat<float> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsFloatNaNConstraint(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that the subject is seen as not a number (<see cref="double.NaN" />).
	/// </summary>
	public static AndOrResult<double, IThat<double>> IsNaN(this IThat<double> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsDoubleNaNConstraint(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that the subject is seen as not a number (not <see langword="null" /> and <see cref="float.NaN" />).
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<float?, IThat<float?>> IsNaN(this IThat<float?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsFloatNaNConstraint(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that the subject is seen as not a number (not <see langword="null" /> and <see cref="double.NaN" />).
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<double?, IThat<double?>> IsNaN(this IThat<double?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsDoubleNaNConstraint(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that the subject is not seen as not a number (not <see cref="float.NaN" />).
	/// </summary>
	public static AndOrResult<float, IThat<float>> IsNotNaN(this IThat<float> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsFloatNaNConstraint(it, grammars).Invert()),
			subject);

	/// <summary>
	///     Verifies that the subject is not seen as not a number (not <see cref="double.NaN" />).
	/// </summary>
	public static AndOrResult<double, IThat<double>> IsNotNaN(this IThat<double> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsDoubleNaNConstraint(it, grammars).Invert()),
			subject);

	/// <summary>
	///     Verifies that the subject is not seen as not a number (<see langword="null" /> or not not <see cref="float.NaN" />
	///     ).
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<float?, IThat<float?>> IsNotNaN(this IThat<float?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsFloatNaNConstraint(it, grammars).Invert()),
			subject);

	/// <summary>
	///     Verifies that the subject is not seen as not a number (<see langword="null" /> or not <see cref="double.NaN" />).
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<double?, IThat<double?>> IsNotNaN(this IThat<double?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsDoubleNaNConstraint(it, grammars).Invert()),
			subject);

	private sealed class IsFloatNaNConstraint(
		string it,
		ExpectationGrammars grammars)
		: ConstraintResult.WithValue<float>(grammars),
			IValueConstraint<float>
	{
		public ConstraintResult IsMetBy(float actual)
		{
			Actual = actual;
			Outcome = float.IsNaN(actual) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(ExpectIsNaN);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(ExpectIsNotNaN);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}

	private sealed class IsDoubleNaNConstraint(
		string it,
		ExpectationGrammars grammars)
		: ConstraintResult.WithValue<double>(grammars),
			IValueConstraint<double>
	{
		public ConstraintResult IsMetBy(double actual)
		{
			Actual = actual;
			Outcome = double.IsNaN(actual) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(ExpectIsNaN);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(ExpectIsNotNaN);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}

	private sealed class NullableIsFloatNaNConstraint(
		string it,
		ExpectationGrammars grammars)
		: ConstraintResult.WithNotNullValue<float?>(it, grammars),
			IValueConstraint<float?>
	{
		public ConstraintResult IsMetBy(float? actual)
		{
			Actual = actual;
			Outcome = actual is not null && float.IsNaN(actual.Value) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(ExpectIsNaN);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(ExpectIsNotNaN);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}

	private sealed class NullableIsDoubleNaNConstraint(
		string it,
		ExpectationGrammars grammars)
		: ConstraintResult.WithNotNullValue<double?>(it, grammars),
			IValueConstraint<double?>
	{
		public ConstraintResult IsMetBy(double? actual)
		{
			Actual = actual;
			Outcome = actual is not null && double.IsNaN(actual.Value) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(ExpectIsNaN);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(ExpectIsNotNaN);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
#endif
}
