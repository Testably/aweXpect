using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;
#if !NET8_0_OR_GREATER
using System;
using aweXpect.SourceGenerators;
#endif

namespace aweXpect;

public static partial class ThatNumber
{
#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the subject is less than or equal to the <paramref name="expected" /> value.
	/// </summary>
	public static NumberToleranceResult<TNumber, IThat<TNumber>> IsLessThanOrEqualTo<TNumber>(
		this IThat<TNumber> subject, TNumber? expected)
		where TNumber : struct, INumber<TNumber>
	{
		expected.ThrowIfNaN("expected value");
		NumberTolerance<TNumber> options = new(CalculateDifference);
		return new NumberToleranceResult<TNumber, IThat<TNumber>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsLessThanOrEqualToConstraint<TNumber>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is less than or equal to the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static NullableNumberToleranceResult<TNumber, IThat<TNumber?>> IsLessThanOrEqualTo<TNumber>(
		this IThat<TNumber?> subject, TNumber? expected)
		where TNumber : struct, INumber<TNumber>
	{
		expected.ThrowIfNaN("expected value");
		NumberTolerance<TNumber> options = new(CalculateDifference);
		return new NullableNumberToleranceResult<TNumber, IThat<TNumber?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsLessThanOrEqualToConstraint<TNumber>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not less than or equal to the <paramref name="unexpected" /> value.
	/// </summary>
	/// <remarks>
	///     A <c>NaN</c> subject is not less than or equal to any value, so it satisfies this expectation
	///     although it fails <c>IsGreaterThan</c>. A <c>NaN</c> <paramref name="unexpected" /> value throws an
	///     <see cref="System.ArgumentOutOfRangeException" />, while a <see langword="null" /> one fails this
	///     expectation as well as <c>IsLessThanOrEqualTo</c>.
	/// </remarks>
	public static NumberToleranceResult<TNumber, IThat<TNumber>> IsNotLessThanOrEqualTo<TNumber>(
		this IThat<TNumber> subject, TNumber? unexpected)
		where TNumber : struct, INumber<TNumber>
	{
		unexpected.ThrowIfNaN("unexpected value");
		NumberTolerance<TNumber> options = new(CalculateDifference);
		return new NumberToleranceResult<TNumber, IThat<TNumber>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsLessThanOrEqualToConstraint<TNumber>(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not less than or equal to the <paramref name="unexpected" /> value.
	/// </summary>
	/// <remarks>
	///     A <c>NaN</c> subject is not less than or equal to any value, so it satisfies this expectation
	///     although it fails <c>IsGreaterThan</c>. A <c>NaN</c> <paramref name="unexpected" /> value throws an
	///     <see cref="System.ArgumentOutOfRangeException" />, while a <see langword="null" /> one fails this
	///     expectation as well as <c>IsLessThanOrEqualTo</c>.
	/// </remarks>
	[GuaranteesNotNull]
	public static NullableNumberToleranceResult<TNumber, IThat<TNumber?>> IsNotLessThanOrEqualTo<TNumber>(
		this IThat<TNumber?> subject, TNumber? unexpected)
		where TNumber : struct, INumber<TNumber>
	{
		unexpected.ThrowIfNaN("unexpected value");
		NumberTolerance<TNumber> options = new(CalculateDifference);
		return new NullableNumberToleranceResult<TNumber, IThat<TNumber?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsLessThanOrEqualToConstraint<TNumber>(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	private sealed class IsLessThanOrEqualToConstraint<TNumber>(
		string it,
		ExpectationGrammars grammars,
		TNumber? expected,
		NumberTolerance<TNumber> options)
		: OrderingConstraint<TNumber>(it, grammars, expected is null),
			IValueConstraint<TNumber>
		where TNumber : struct, INumber<TNumber>
	{
		public ConstraintResult IsMetBy(TNumber actual)
		{
			Actual = actual;
			Outcome = options.IsLessThanOrEqualTo(actual, expected) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is less than or equal to ", "are less than or equal to "));
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(options);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
			AppendDifference(stringBuilder, Actual, expected);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not less than or equal to ", "are not less than or equal to "));
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(options);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}

	private sealed class NullableIsLessThanOrEqualToConstraint<TNumber>(
		string it,
		ExpectationGrammars grammars,
		TNumber? expected,
		NumberTolerance<TNumber> options)
		: OrderingConstraint<TNumber?>(it, grammars, expected is null),
			IValueConstraint<TNumber?>
		where TNumber : struct, INumber<TNumber>
	{
		public ConstraintResult IsMetBy(TNumber? actual)
		{
			Actual = actual;
			Outcome = options.IsLessThanOrEqualTo(actual, expected) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is less than or equal to ", "are less than or equal to "));
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(options);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
			AppendDifference(stringBuilder, Actual, expected);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not less than or equal to ", "are not less than or equal to "));
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(options);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
#else
	private const string IsLessThanOrEqualToSummary =
		"Verifies that the subject is less than or equal to the <paramref name=\"expected\" /> value.";

	private const string IsNotLessThanOrEqualToSummary =
		"Verifies that the subject is not less than or equal to the <paramref name=\"unexpected\" /> value.";

	private const string IsNotLessThanOrEqualToRemarks =
		"A <c>NaN</c> subject is not less than or equal to any value, so it satisfies this expectation\n" +
		"although it fails <c>IsGreaterThan</c>. A <c>NaN</c> <paramref name=\"unexpected\" /> value throws an\n" +
		"<see cref=\"System.ArgumentOutOfRangeException\" />, while a <see langword=\"null\" /> one fails this\n" +
		"expectation as well as <c>IsLessThanOrEqualTo</c>.";

	[CreateCollectionExpectation("Is{Not}LessThanOrEqualTo", Factory = typeof(NumberToleranceFactory),
		Summary = IsLessThanOrEqualToSummary, NegatedSummary = IsNotLessThanOrEqualToSummary,
		NegatedRemarks = IsNotLessThanOrEqualToRemarks)]
	internal static NumberToleranceResult<TNumber, IThat<TNumber>> IsLessThanOrEqualToCore<TNumber>(
		IThat<TNumber> subject,
		TNumber? expected,
		NumberTolerance<TNumber> options,
		bool negated)
		where TNumber : struct, IComparable<TNumber>
	{
		ThrowIfNaN(expected, negated);
		return new NumberToleranceResult<TNumber, IThat<TNumber>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsLessThanOrEqualToConstraint<TNumber>(it, grammars, expected, options).InvertIf(negated)),
			subject,
			options);
	}

	[CreateCollectionExpectation("Is{Not}LessThanOrEqualTo", Factory = typeof(NumberToleranceFactory), GuaranteesNotNull = true,
		Summary = IsLessThanOrEqualToSummary, NegatedSummary = IsNotLessThanOrEqualToSummary,
		NegatedRemarks = IsNotLessThanOrEqualToRemarks)]
	internal static NullableNumberToleranceResult<TNumber, IThat<TNumber?>> IsLessThanOrEqualToForNullableCore<TNumber>(
		IThat<TNumber?> subject,
		TNumber? expected,
		NumberTolerance<TNumber> options,
		bool negated)
		where TNumber : struct, IComparable<TNumber>
	{
		ThrowIfNaN(expected, negated);
		return new NullableNumberToleranceResult<TNumber, IThat<TNumber?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsLessThanOrEqualToConstraint<TNumber>(it, grammars, expected, options).InvertIf(negated)),
			subject,
			options);
	}

	private sealed class IsLessThanOrEqualToConstraint<TNumber>(
		string it,
		ExpectationGrammars grammars,
		TNumber? expected,
		NumberTolerance<TNumber> options)
		: OrderingConstraint<TNumber>(it, grammars, expected is null),
			IValueConstraint<TNumber>
		where TNumber : struct, IComparable<TNumber>
	{
		public ConstraintResult IsMetBy(TNumber actual)
		{
			Actual = actual;
			Outcome = options.IsLessThanOrEqualTo(actual, expected) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is less than or equal to ", "are less than or equal to "));
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(options);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
			AppendDifference(stringBuilder, Actual, expected, options);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not less than or equal to ", "are not less than or equal to "));
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(options);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}

	private sealed class NullableIsLessThanOrEqualToConstraint<TNumber>(
		string it,
		ExpectationGrammars grammars,
		TNumber? expected,
		NumberTolerance<TNumber> options)
		: OrderingConstraint<TNumber?>(it, grammars, expected is null),
			IValueConstraint<TNumber?>
		where TNumber : struct, IComparable<TNumber>
	{
		public ConstraintResult IsMetBy(TNumber? actual)
		{
			Actual = actual;
			Outcome = options.IsLessThanOrEqualTo(actual, expected) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is less than or equal to ", "are less than or equal to "));
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(options);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
			AppendDifference(stringBuilder, Actual, expected, options);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not less than or equal to ", "are not less than or equal to "));
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(options);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
#endif
}
