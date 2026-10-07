using System.Collections.Generic;
using System.Linq;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;
#if NET8_0_OR_GREATER
using System.Runtime.CompilerServices;

#else
using System;
using aweXpect.SourceGenerators;
#endif

namespace aweXpect;

public static partial class ThatNumber
{
#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NumberToleranceResult<TNumber, IThat<TNumber>> IsOneOf<TNumber>(
		this IThat<TNumber> subject,
		params TNumber?[] expected)
		where TNumber : struct, INumber<TNumber>
	{
		IEnumerable<TNumber?> expectedValues = expected.ToNonEmptyValues(false);
		NumberTolerance<TNumber> options = new(CalculateDifference);
		return new NumberToleranceResult<TNumber, IThat<TNumber>>(
			subject.Get().ExpectationBuilder.AddConstraint((ExpectedValues: expectedValues, Options: options),
				static (state, it, grammars) =>
					new IsOneOfConstraintWithNullable<TNumber>(it, grammars, state.ExpectedValues,
						null, state.Options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<TNumber, IThat<TNumber?>> IsOneOf<TNumber>(
		this IThat<TNumber?> subject,
		params TNumber?[] expected)
		where TNumber : struct, INumber<TNumber>
	{
		IEnumerable<TNumber?> expectedValues = expected.ToNonEmptyValues(false);
		NumberTolerance<TNumber> options = new(CalculateDifference);
		return new NullableNumberToleranceResult<TNumber, IThat<TNumber?>>(
			subject.Get().ExpectationBuilder.AddConstraint((ExpectedValues: expectedValues, Options: options),
				static (state, it, grammars) =>
					new NullableIsOneOfConstraintWithNullable<TNumber>(it, grammars, state.ExpectedValues,
						null, state.Options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NumberToleranceResult<TNumber, IThat<TNumber>> IsOneOf<TNumber>(
		this IThat<TNumber> subject,
		IEnumerable<TNumber> expected,
		[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
		where TNumber : struct, INumber<TNumber>
	{
		IEnumerable<TNumber> expectedValues = expected.ToNonEmptyValues(false);
		NumberTolerance<TNumber> options = new(CalculateDifference);
		return new NumberToleranceResult<TNumber, IThat<TNumber>>(
			subject.Get().ExpectationBuilder.AddConstraint(
				(ExpectedValues: expectedValues, DoNotPopulateThisValue: doNotPopulateThisValue, Options: options),
				static (state, it, grammars) =>
					new IsOneOfConstraint<TNumber>(it, grammars, state.ExpectedValues,
						state.DoNotPopulateThisValue, state.Options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<TNumber, IThat<TNumber?>> IsOneOf<TNumber>(
		this IThat<TNumber?> subject,
		IEnumerable<TNumber> expected,
		[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
		where TNumber : struct, INumber<TNumber>
	{
		IEnumerable<TNumber> expectedValues = expected.ToNonEmptyValues(false);
		NumberTolerance<TNumber> options = new(CalculateDifference);
		return new NullableNumberToleranceResult<TNumber, IThat<TNumber?>>(
			subject.Get().ExpectationBuilder.AddConstraint(
				(ExpectedValues: expectedValues, DoNotPopulateThisValue: doNotPopulateThisValue, Options: options),
				static (state, it, grammars) =>
					new NullableIsOneOfConstraint<TNumber>(it, grammars, state.ExpectedValues,
						state.DoNotPopulateThisValue, state.Options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NumberToleranceResult<TNumber, IThat<TNumber>> IsOneOf<TNumber>(
		this IThat<TNumber> subject,
		IEnumerable<TNumber?> expected,
		[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
		where TNumber : struct, INumber<TNumber>
	{
		IEnumerable<TNumber?> expectedValues = expected.ToNonEmptyValues(false);
		NumberTolerance<TNumber> options = new(CalculateDifference);
		return new NumberToleranceResult<TNumber, IThat<TNumber>>(
			subject.Get().ExpectationBuilder.AddConstraint(
				(ExpectedValues: expectedValues, DoNotPopulateThisValue: doNotPopulateThisValue, Options: options),
				static (state, it, grammars) =>
					new IsOneOfConstraintWithNullable<TNumber>(it, grammars, state.ExpectedValues,
						state.DoNotPopulateThisValue, state.Options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<TNumber, IThat<TNumber?>> IsOneOf<TNumber>(
		this IThat<TNumber?> subject,
		IEnumerable<TNumber?> expected,
		[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
		where TNumber : struct, INumber<TNumber>
	{
		IEnumerable<TNumber?> expectedValues = expected.ToNonEmptyValues(false);
		NumberTolerance<TNumber> options = new(CalculateDifference);
		return new NullableNumberToleranceResult<TNumber, IThat<TNumber?>>(
			subject.Get().ExpectationBuilder.AddConstraint(
				(ExpectedValues: expectedValues, DoNotPopulateThisValue: doNotPopulateThisValue, Options: options),
				static (state, it, grammars) =>
					new NullableIsOneOfConstraintWithNullable<TNumber>(it, grammars, state.ExpectedValues,
						state.DoNotPopulateThisValue, state.Options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NumberToleranceResult<TNumber, IThat<TNumber>> IsNotOneOf<TNumber>(
		this IThat<TNumber> subject,
		params TNumber?[] unexpected)
		where TNumber : struct, INumber<TNumber>
	{
		IEnumerable<TNumber?> unexpectedValues = unexpected.ToNonEmptyValues(true);
		NumberTolerance<TNumber> options = new(CalculateDifference);
		return new NumberToleranceResult<TNumber, IThat<TNumber>>(
			subject.Get().ExpectationBuilder.AddConstraint((UnexpectedValues: unexpectedValues, Options: options),
				static (state, it, grammars) =>
					new IsOneOfConstraintWithNullable<TNumber>(it, grammars, state.UnexpectedValues,
						null, state.Options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<TNumber, IThat<TNumber?>> IsNotOneOf<TNumber>(
		this IThat<TNumber?> subject,
		params TNumber?[] unexpected)
		where TNumber : struct, INumber<TNumber>
	{
		IEnumerable<TNumber?> unexpectedValues = unexpected.ToNonEmptyValues(true);
		NumberTolerance<TNumber> options = new(CalculateDifference);
		return new NullableNumberToleranceResult<TNumber, IThat<TNumber?>>(
			subject.Get().ExpectationBuilder.AddConstraint((UnexpectedValues: unexpectedValues, Options: options),
				static (state, it, grammars) =>
					new NullableIsOneOfConstraintWithNullable<TNumber>(it, grammars, state.UnexpectedValues,
							null, state.Options)
						.Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NumberToleranceResult<TNumber, IThat<TNumber>> IsNotOneOf<TNumber>(
		this IThat<TNumber> subject,
		IEnumerable<TNumber> unexpected,
		[CallerArgumentExpression("unexpected")]
		string doNotPopulateThisValue = "")
		where TNumber : struct, INumber<TNumber>
	{
		IEnumerable<TNumber> unexpectedValues = unexpected.ToNonEmptyValues(true);
		NumberTolerance<TNumber> options = new(CalculateDifference);
		return new NumberToleranceResult<TNumber, IThat<TNumber>>(
			subject.Get().ExpectationBuilder.AddConstraint(
				(UnexpectedValues: unexpectedValues, DoNotPopulateThisValue: doNotPopulateThisValue, Options: options),
				static (state, it, grammars) =>
					new IsOneOfConstraint<TNumber>(it, grammars, state.UnexpectedValues,
							state.DoNotPopulateThisValue, state.Options)
						.Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<TNumber, IThat<TNumber?>> IsNotOneOf<TNumber>(
		this IThat<TNumber?> subject,
		IEnumerable<TNumber> unexpected,
		[CallerArgumentExpression("unexpected")]
		string doNotPopulateThisValue = "")
		where TNumber : struct, INumber<TNumber>
	{
		IEnumerable<TNumber> unexpectedValues = unexpected.ToNonEmptyValues(true);
		NumberTolerance<TNumber> options = new(CalculateDifference);
		return new NullableNumberToleranceResult<TNumber, IThat<TNumber?>>(
			subject.Get().ExpectationBuilder.AddConstraint(
				(UnexpectedValues: unexpectedValues, DoNotPopulateThisValue: doNotPopulateThisValue, Options: options),
				static (state, it, grammars) =>
					new NullableIsOneOfConstraint<TNumber>(it, grammars, state.UnexpectedValues,
							state.DoNotPopulateThisValue, state.Options)
						.Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NumberToleranceResult<TNumber, IThat<TNumber>> IsNotOneOf<TNumber>(
		this IThat<TNumber> subject,
		IEnumerable<TNumber?> unexpected,
		[CallerArgumentExpression("unexpected")]
		string doNotPopulateThisValue = "")
		where TNumber : struct, INumber<TNumber>
	{
		IEnumerable<TNumber?> unexpectedValues = unexpected.ToNonEmptyValues(true);
		NumberTolerance<TNumber> options = new(CalculateDifference);
		return new NumberToleranceResult<TNumber, IThat<TNumber>>(
			subject.Get().ExpectationBuilder.AddConstraint(
				(UnexpectedValues: unexpectedValues, DoNotPopulateThisValue: doNotPopulateThisValue, Options: options),
				static (state, it, grammars) =>
					new IsOneOfConstraintWithNullable<TNumber>(it, grammars, state.UnexpectedValues,
							state.DoNotPopulateThisValue, state.Options)
						.Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<TNumber, IThat<TNumber?>> IsNotOneOf<TNumber>(
		this IThat<TNumber?> subject,
		IEnumerable<TNumber?> unexpected,
		[CallerArgumentExpression("unexpected")]
		string doNotPopulateThisValue = "")
		where TNumber : struct, INumber<TNumber>
	{
		IEnumerable<TNumber?> unexpectedValues = unexpected.ToNonEmptyValues(true);
		NumberTolerance<TNumber> options = new(CalculateDifference);
		return new NullableNumberToleranceResult<TNumber, IThat<TNumber?>>(
			subject.Get().ExpectationBuilder.AddConstraint(
				(UnexpectedValues: unexpectedValues, DoNotPopulateThisValue: doNotPopulateThisValue, Options: options),
				static (state, it, grammars) =>
					new NullableIsOneOfConstraintWithNullable<TNumber>(it, grammars, state.UnexpectedValues,
							state.DoNotPopulateThisValue, state.Options)
						.Invert()),
			subject,
			options);
	}

	private sealed class IsOneOfConstraint<TNumber>(
		string it,
		ExpectationGrammars grammars,
		IEnumerable<TNumber> expected,
		string? expectedExpression,
		NumberTolerance<TNumber> options)
		: ConstraintResult.WithValue<TNumber>(it, grammars),
			IValueConstraint<TNumber>
		where TNumber : struct, INumber<TNumber>
	{
		public ConstraintResult IsMetBy(TNumber actual)
		{
			Actual = actual;
			Outcome = expected.Any(value => options.IsWithinTolerance(actual, value))
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
			=> contexts.AddExpectedValuesContext(expectedExpression, expected, Grammars.IsNegated());

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is one of ", "are one of "));
			stringBuilder.Append(expectedExpression?.TrimCommonWhiteSpace() ?? Formatter.Format(expected));
			stringBuilder.Append(options);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
			AppendDifferenceToClosest(stringBuilder, Actual, expected.Select(value => (TNumber?)value), options);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not one of ", "are not one of "));
			stringBuilder.Append(expectedExpression?.TrimCommonWhiteSpace() ?? Formatter.Format(expected));
			stringBuilder.Append(options);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}

	private sealed class NullableIsOneOfConstraint<TNumber>(
		string it,
		ExpectationGrammars grammars,
		IEnumerable<TNumber> expected,
		string? expectedExpression,
		NumberTolerance<TNumber> options)
		: ConstraintResult.WithValue<TNumber?>(it, grammars),
			IValueConstraint<TNumber?>
		where TNumber : struct, INumber<TNumber>
	{
		public ConstraintResult IsMetBy(TNumber? actual)
		{
			Actual = actual;
			Outcome = expected.Any(value => options.IsWithinTolerance(actual, value))
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
			=> contexts.AddExpectedValuesContext(expectedExpression, expected, Grammars.IsNegated());

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is one of ", "are one of "));
			stringBuilder.Append(expectedExpression?.TrimCommonWhiteSpace() ?? Formatter.Format(expected));
			stringBuilder.Append(options);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
			AppendDifferenceToClosest(stringBuilder, Actual, expected.Select(value => (TNumber?)value), options);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not one of ", "are not one of "));
			stringBuilder.Append(expectedExpression?.TrimCommonWhiteSpace() ?? Formatter.Format(expected));
			stringBuilder.Append(options);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}

	private sealed class IsOneOfConstraintWithNullable<TNumber>(
		string it,
		ExpectationGrammars grammars,
		IEnumerable<TNumber?> expected,
		string? expectedExpression,
		NumberTolerance<TNumber> options)
		: ConstraintResult.WithValue<TNumber>(it, grammars),
			IValueConstraint<TNumber>
		where TNumber : struct, INumber<TNumber>
	{
		public ConstraintResult IsMetBy(TNumber actual)
		{
			Actual = actual;
			Outcome = expected.Any(value => options.IsWithinTolerance(actual, value))
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
			=> contexts.AddExpectedValuesContext(expectedExpression, expected, Grammars.IsNegated());

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is one of ", "are one of "));
			stringBuilder.Append(expectedExpression?.TrimCommonWhiteSpace() ?? Formatter.Format(expected));
			stringBuilder.Append(options);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
			AppendDifferenceToClosest(stringBuilder, Actual, expected, options);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not one of ", "are not one of "));
			stringBuilder.Append(expectedExpression?.TrimCommonWhiteSpace() ?? Formatter.Format(expected));
			stringBuilder.Append(options);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}

	private sealed class NullableIsOneOfConstraintWithNullable<TNumber>(
		string it,
		ExpectationGrammars grammars,
		IEnumerable<TNumber?> expected,
		string? expectedExpression,
		NumberTolerance<TNumber> options)
		: ConstraintResult.WithValue<TNumber?>(it, grammars),
			IValueConstraint<TNumber?>
		where TNumber : struct, INumber<TNumber>
	{
		public ConstraintResult IsMetBy(TNumber? actual)
		{
			Actual = actual;
			Outcome = expected.Any(value => options.IsWithinTolerance(actual, value))
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
			=> contexts.AddExpectedValuesContext(expectedExpression, expected, Grammars.IsNegated());

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is one of ", "are one of "));
			stringBuilder.Append(expectedExpression?.TrimCommonWhiteSpace() ?? Formatter.Format(expected));
			stringBuilder.Append(options);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
			AppendDifferenceToClosest(stringBuilder, Actual, expected, options);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not one of ", "are not one of "));
			stringBuilder.Append(expectedExpression?.TrimCommonWhiteSpace() ?? Formatter.Format(expected));
			stringBuilder.Append(options);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
#else
	private const string IsOneOfSummary =
		"Verifies that the subject is one of the <paramref name=\"expected\" /> values.";

	private const string IsNotOneOfSummary =
		"Verifies that the subject is not one of the <paramref name=\"unexpected\" /> values.";

	[CreateExpectationFamily("Is{Not}OneOf", Factory = typeof(NumberToleranceFactory),
		Summary = IsOneOfSummary, NegatedSummary = IsNotOneOfSummary)]
	[CreateExpectationFamily("Is{Not}OneOf", Factory = typeof(NumberToleranceFactory), Params = true,
		Summary = IsOneOfSummary, NegatedSummary = IsNotOneOfSummary)]
	internal static NumberToleranceResult<TNumber, IThat<TNumber>> IsOneOfWithNullableValuesCore<TNumber>(
		IThat<TNumber> subject,
		IEnumerable<TNumber?> expected,
		NumberTolerance<TNumber> options,
		string? expectedExpression,
		bool negated)
		where TNumber : struct, IComparable<TNumber>
	{
		IEnumerable<TNumber?> expectedValues = expected.ToNonEmptyValues(negated);
		return new NumberToleranceResult<TNumber, IThat<TNumber>>(
			subject.Get().ExpectationBuilder.AddConstraint(
				(ExpectedValues: expectedValues, ExpectedExpression: expectedExpression, Options: options,
					Negated: negated),
				static (state, it, grammars) =>
					new IsOneOfConstraintWithNullable<TNumber>(it, grammars, state.ExpectedValues,
						state.ExpectedExpression, state.Options)
						.InvertIf(state.Negated)),
			subject,
			options);
	}

	[CreateExpectationFamily("Is{Not}OneOf", Factory = typeof(NumberToleranceFactory),
		Summary = IsOneOfSummary, NegatedSummary = IsNotOneOfSummary)]
	internal static NumberToleranceResult<TNumber, IThat<TNumber>> IsOneOfCore<TNumber>(
		IThat<TNumber> subject,
		IEnumerable<TNumber> expected,
		NumberTolerance<TNumber> options,
		string? expectedExpression,
		bool negated)
		where TNumber : struct, IComparable<TNumber>
	{
		IEnumerable<TNumber> expectedValues = expected.ToNonEmptyValues(negated);
		return new NumberToleranceResult<TNumber, IThat<TNumber>>(
			subject.Get().ExpectationBuilder.AddConstraint(
				(ExpectedValues: expectedValues, ExpectedExpression: expectedExpression, Options: options,
					Negated: negated),
				static (state, it, grammars) =>
					new IsOneOfConstraint<TNumber>(it, grammars, state.ExpectedValues,
						state.ExpectedExpression, state.Options)
						.InvertIf(state.Negated)),
			subject,
			options);
	}

	[CreateExpectationFamily("Is{Not}OneOf", Factory = typeof(NumberToleranceFactory),
		Summary = IsOneOfSummary, NegatedSummary = IsNotOneOfSummary)]
	[CreateExpectationFamily("Is{Not}OneOf", Factory = typeof(NumberToleranceFactory), Params = true,
		Summary = IsOneOfSummary, NegatedSummary = IsNotOneOfSummary)]
	internal static NullableNumberToleranceResult<TNumber, IThat<TNumber?>>
		IsOneOfForNullableWithNullableValuesCore<TNumber>(
			IThat<TNumber?> subject,
			IEnumerable<TNumber?> expected,
			NumberTolerance<TNumber> options,
			string? expectedExpression,
			bool negated)
		where TNumber : struct, IComparable<TNumber>
	{
		IEnumerable<TNumber?> expectedValues = expected.ToNonEmptyValues(negated);
		return new NullableNumberToleranceResult<TNumber, IThat<TNumber?>>(
			subject.Get().ExpectationBuilder.AddConstraint(
				(ExpectedValues: expectedValues, ExpectedExpression: expectedExpression, Options: options,
					Negated: negated),
				static (state, it, grammars) =>
					new NullableIsOneOfConstraintWithNullable<TNumber>(it, grammars, state.ExpectedValues,
						state.ExpectedExpression, state.Options)
						.InvertIf(state.Negated)),
			subject,
			options);
	}

	[CreateExpectationFamily("Is{Not}OneOf", Factory = typeof(NumberToleranceFactory),
		Summary = IsOneOfSummary, NegatedSummary = IsNotOneOfSummary)]
	internal static NullableNumberToleranceResult<TNumber, IThat<TNumber?>> IsOneOfForNullableCore<TNumber>(
		IThat<TNumber?> subject,
		IEnumerable<TNumber> expected,
		NumberTolerance<TNumber> options,
		string? expectedExpression,
		bool negated)
		where TNumber : struct, IComparable<TNumber>
	{
		IEnumerable<TNumber> expectedValues = expected.ToNonEmptyValues(negated);
		return new NullableNumberToleranceResult<TNumber, IThat<TNumber?>>(
			subject.Get().ExpectationBuilder.AddConstraint(
				(ExpectedValues: expectedValues, ExpectedExpression: expectedExpression, Options: options,
					Negated: negated),
				static (state, it, grammars) =>
					new NullableIsOneOfConstraint<TNumber>(it, grammars, state.ExpectedValues,
						state.ExpectedExpression, state.Options)
						.InvertIf(state.Negated)),
			subject,
			options);
	}

	private sealed class IsOneOfConstraint<TNumber>(
		string it,
		ExpectationGrammars grammars,
		IEnumerable<TNumber> expected,
		string? expectedExpression,
		NumberTolerance<TNumber> options)
		: ConstraintResult.WithValue<TNumber>(it, grammars),
			IValueConstraint<TNumber>
		where TNumber : struct, IComparable<TNumber>
	{
		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
			=> contexts.AddExpectedValuesContext(expectedExpression, expected, Grammars.IsNegated());

		public ConstraintResult IsMetBy(TNumber actual)
		{
			Actual = actual;
			Outcome = expected.Any(value => options.IsWithinTolerance(actual, value))
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is one of ", "are one of "));
			stringBuilder.Append(expectedExpression?.TrimCommonWhiteSpace()
			                     ?? ValueFormatters.Format(Formatter, expected));
			stringBuilder.Append(options);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
			AppendDifferenceToClosest(stringBuilder, Actual, expected.Select(value => (TNumber?)value), options);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not one of ", "are not one of "));
			stringBuilder.Append(expectedExpression?.TrimCommonWhiteSpace()
			                     ?? ValueFormatters.Format(Formatter, expected));
			stringBuilder.Append(options);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}

	private sealed class NullableIsOneOfConstraint<TNumber>(
		string it,
		ExpectationGrammars grammars,
		IEnumerable<TNumber> expected,
		string? expectedExpression,
		NumberTolerance<TNumber> options)
		: ConstraintResult.WithValue<TNumber?>(it, grammars),
			IValueConstraint<TNumber?>
		where TNumber : struct, IComparable<TNumber>
	{
		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
			=> contexts.AddExpectedValuesContext(expectedExpression, expected, Grammars.IsNegated());

		public ConstraintResult IsMetBy(TNumber? actual)
		{
			Actual = actual;
			Outcome = expected.Any(value => options.IsWithinTolerance(actual, value))
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is one of ", "are one of "));
			stringBuilder.Append(expectedExpression?.TrimCommonWhiteSpace()
			                     ?? ValueFormatters.Format(Formatter, expected));
			stringBuilder.Append(options);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
			AppendDifferenceToClosest(stringBuilder, Actual, expected.Select(value => (TNumber?)value), options);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not one of ", "are not one of "));
			stringBuilder.Append(expectedExpression?.TrimCommonWhiteSpace()
			                     ?? ValueFormatters.Format(Formatter, expected));
			stringBuilder.Append(options);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}

	private sealed class IsOneOfConstraintWithNullable<TNumber>(
		string it,
		ExpectationGrammars grammars,
		IEnumerable<TNumber?> expected,
		string? expectedExpression,
		NumberTolerance<TNumber> options)
		: ConstraintResult.WithValue<TNumber>(it, grammars),
			IValueConstraint<TNumber>
		where TNumber : struct, IComparable<TNumber>
	{
		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
			=> contexts.AddExpectedValuesContext(expectedExpression, expected, Grammars.IsNegated());

		public ConstraintResult IsMetBy(TNumber actual)
		{
			Actual = actual;
			Outcome = expected.Any(value => options.IsWithinTolerance(actual, value))
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is one of ", "are one of "));
			stringBuilder.Append(expectedExpression?.TrimCommonWhiteSpace()
			                     ?? ValueFormatters.Format(Formatter, expected));
			stringBuilder.Append(options);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
			AppendDifferenceToClosest(stringBuilder, Actual, expected, options);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not one of ", "are not one of "));
			stringBuilder.Append(expectedExpression?.TrimCommonWhiteSpace()
			                     ?? ValueFormatters.Format(Formatter, expected));
			stringBuilder.Append(options);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}

	private sealed class NullableIsOneOfConstraintWithNullable<TNumber>(
		string it,
		ExpectationGrammars grammars,
		IEnumerable<TNumber?> expected,
		string? expectedExpression,
		NumberTolerance<TNumber> options)
		: ConstraintResult.WithValue<TNumber?>(it, grammars),
			IValueConstraint<TNumber?>
		where TNumber : struct, IComparable<TNumber>
	{
		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
			=> contexts.AddExpectedValuesContext(expectedExpression, expected, Grammars.IsNegated());

		public ConstraintResult IsMetBy(TNumber? actual)
		{
			Actual = actual;
			Outcome = expected.Any(value => options.IsWithinTolerance(actual, value))
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is one of ", "are one of "));
			stringBuilder.Append(expectedExpression?.TrimCommonWhiteSpace()
			                     ?? ValueFormatters.Format(Formatter, expected));
			stringBuilder.Append(options);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
			AppendDifferenceToClosest(stringBuilder, Actual, expected, options);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not one of ", "are not one of "));
			stringBuilder.Append(expectedExpression?.TrimCommonWhiteSpace()
			                     ?? ValueFormatters.Format(Formatter, expected));
			stringBuilder.Append(options);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
#endif
}
