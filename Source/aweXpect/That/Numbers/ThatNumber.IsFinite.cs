using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatNumber
{
	private const string ExpectIsFinite = "is finite";
	private const string ExpectIsNotFinite = "is not finite";

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the subject is seen as finite.
	/// </summary>
	/// <remarks>
	///     Finite means neither infinity nor not a number (NaN).
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<TNumber, IThat<TNumber>> IsFinite<TNumber>(this IThat<TNumber> source)
		where TNumber : struct, IFloatingPoint<TNumber>
		=> new(source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsFiniteConstraint<TNumber>(it, grammars)),
			source);

	/// <summary>
	///     Verifies that the subject is seen as finite.
	/// </summary>
	/// <remarks>
	///     Finite means neither infinity nor not a number (NaN) nor <see langword="null" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<TNumber?, IThat<TNumber?>> IsFinite<TNumber>(this IThat<TNumber?> source)
		where TNumber : struct, IFloatingPoint<TNumber>
		=> new(source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsFiniteConstraint<TNumber>(it, grammars)),
			source);

	/// <summary>
	///     Verifies that the subject is not seen as finite.
	/// </summary>
	/// <remarks>
	///     Not finite means either infinity or not a number (NaN).
	/// </remarks>
	public static AndOrResult<TNumber, IThat<TNumber>> IsNotFinite<TNumber>(this IThat<TNumber> source)
		where TNumber : struct, IFloatingPoint<TNumber>
		=> new(source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsFiniteConstraint<TNumber>(it, grammars).Invert()),
			source);

	/// <summary>
	///     Verifies that the subject is not seen as finite.
	/// </summary>
	/// <remarks>
	///     Not finite means either infinity or not a number (NaN) or <see langword="null" />.
	/// </remarks>
	public static AndOrResult<TNumber?, IThat<TNumber?>> IsNotFinite<TNumber>(this IThat<TNumber?> source)
		where TNumber : struct, IFloatingPoint<TNumber>
		=> new(source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsFiniteConstraint<TNumber>(it, grammars).Invert()),
			source);

	private sealed class IsFiniteConstraint<TNumber>(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithValue<TNumber>(grammars),
			IValueConstraint<TNumber>
		where TNumber : struct, IFloatingPoint<TNumber>
	{
		public ConstraintResult IsMetBy(TNumber actual)
		{
			Actual = actual;
			Outcome = !TNumber.IsInfinity(actual) && !TNumber.IsNaN(actual) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(ExpectIsFinite);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(ExpectIsNotFinite);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}

	private sealed class NullableIsFiniteConstraint<TNumber>(
		string it,
		ExpectationGrammars grammars)
		: ConstraintResult.WithValue<TNumber?>(grammars),
			IValueConstraint<TNumber?>
		where TNumber : struct, IFloatingPoint<TNumber>
	{
		public ConstraintResult IsMetBy(TNumber? actual)
		{
			Actual = actual;
			Outcome = actual is not null && !TNumber.IsInfinity(actual.Value) && !TNumber.IsNaN(actual.Value)
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(ExpectIsFinite);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(ExpectIsNotFinite);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
#else
	/// <summary>
	///     Verifies that the subject is seen as finite (neither <see cref="float.IsInfinity" />
	///     nor <see cref="float.IsNaN" />).
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<float, IThat<float>> IsFinite(
		this IThat<float> source)
		=> new(source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsFloatFiniteConstraint(it, grammars)),
			source);

	/// <summary>
	///     Verifies that the subject is seen as finite (neither <see cref="double.IsInfinity" />
	///     nor <see cref="double.IsNaN" />).
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<double, IThat<double>> IsFinite(
		this IThat<double> source)
		=> new(source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsDoubleFiniteConstraint(it, grammars)),
			source);

	/// <summary>
	///     Verifies that the subject is seen as finite (neither <see cref="float.IsInfinity" /> nor
	///     <see cref="float.IsNaN" /> nor <see langword="null" />).
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<float, IThat<float?>> IsFinite(
		this IThat<float?> source)
		=> new(source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsFloatFiniteConstraint(it, grammars)),
			source);

	/// <summary>
	///     Verifies that the subject is seen as finite (neither <see cref="double.IsInfinity" /> nor
	///     <see cref="double.IsNaN" /> nor <see langword="null" />).
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<double, IThat<double?>> IsFinite(
		this IThat<double?> source)
		=> new(source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsDoubleFiniteConstraint(it, grammars)),
			source);

	/// <summary>
	///     Verifies that the subject is not seen as finite (either <see cref="float.IsInfinity" /> or
	///     <see cref="float.IsNaN" />).
	/// </summary>
	public static AndOrResult<float, IThat<float>> IsNotFinite(
		this IThat<float> source)
		=> new(source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsFloatFiniteConstraint(it, grammars).Invert()),
			source);

	/// <summary>
	///     Verifies that the subject is not seen as finite (either <see cref="double.IsInfinity" /> or
	///     <see cref="double.IsNaN" />).
	/// </summary>
	public static AndOrResult<double, IThat<double>> IsNotFinite(
		this IThat<double> source)
		=> new(source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsDoubleFiniteConstraint(it, grammars).Invert()),
			source);

	/// <summary>
	///     Verifies that the subject is not seen as finite (either <see cref="float.IsInfinity" /> or
	///     <see cref="float.IsNaN" /> or <see langword="null" />).
	/// </summary>
	public static AndOrResult<float?, IThat<float?>> IsNotFinite(
		this IThat<float?> source)
		=> new(source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsFloatFiniteConstraint(it, grammars).Invert()),
			source);

	/// <summary>
	///     Verifies that the subject is not seen as finite (either <see cref="double.IsInfinity" /> or
	///     <see cref="double.IsNaN" /> or <see langword="null" />).
	/// </summary>
	public static AndOrResult<double?, IThat<double?>> IsNotFinite(
		this IThat<double?> source)
		=> new(source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsDoubleFiniteConstraint(it, grammars).Invert()),
			source);

	private sealed class IsFloatFiniteConstraint(
		string it,
		ExpectationGrammars grammars)
		: ConstraintResult.WithValue<float>(grammars),
			IValueConstraint<float>
	{
		public ConstraintResult IsMetBy(float actual)
		{
			Actual = actual;
			Outcome = !float.IsInfinity(actual) && !float.IsNaN(actual) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(ExpectIsFinite);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(ExpectIsNotFinite);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}

	private sealed class IsDoubleFiniteConstraint(
		string it,
		ExpectationGrammars grammars)
		: ConstraintResult.WithValue<double>(grammars),
			IValueConstraint<double>
	{
		public ConstraintResult IsMetBy(double actual)
		{
			Actual = actual;
			Outcome = !double.IsInfinity(actual) && !double.IsNaN(actual) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(ExpectIsFinite);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(ExpectIsNotFinite);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}

	private sealed class NullableIsFloatFiniteConstraint(
		string it,
		ExpectationGrammars grammars)
		: ConstraintResult.WithValue<float?>(grammars),
			IValueConstraint<float?>
	{
		public ConstraintResult IsMetBy(float? actual)
		{
			Actual = actual;
			Outcome = actual is not null && !float.IsInfinity(actual.Value) && !float.IsNaN(actual.Value)
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(ExpectIsFinite);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(ExpectIsNotFinite);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}

	private sealed class NullableIsDoubleFiniteConstraint(
		string it,
		ExpectationGrammars grammars)
		: ConstraintResult.WithValue<double?>(grammars),
			IValueConstraint<double?>
	{
		public ConstraintResult IsMetBy(double? actual)
		{
			Actual = actual;
			Outcome = actual is not null && !double.IsInfinity(actual.Value) && !double.IsNaN(actual.Value)
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(ExpectIsFinite);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(ExpectIsNotFinite);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
#endif
}
