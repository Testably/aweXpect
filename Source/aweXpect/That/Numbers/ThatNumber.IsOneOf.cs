using System.Collections.Generic;
using System.Linq;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
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
		return new NumberToleranceResult<TNumber, IThat<TNumber>>(
			subject.Get().ExpectationBuilder.AddConstraint(expectedValues,
				static (expectedValues, it, grammars) =>
					new IsOneOfConstraintWithNullable<TNumber>(it, grammars, expectedValues,
						null),
				out IsOneOfConstraintWithNullable<TNumber> constraint),
			subject,
			constraint);
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
		return new NullableNumberToleranceResult<TNumber, IThat<TNumber?>>(
			subject.Get().ExpectationBuilder.AddConstraint(expectedValues,
				static (expectedValues, it, grammars) =>
					new NullableIsOneOfConstraintWithNullable<TNumber>(it, grammars, expectedValues,
						null),
				out NullableIsOneOfConstraintWithNullable<TNumber> constraint),
			subject,
			constraint);
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
		return new NumberToleranceResult<TNumber, IThat<TNumber>>(
			subject.Get().ExpectationBuilder.AddConstraint(
				(ExpectedValues: expectedValues, DoNotPopulateThisValue: doNotPopulateThisValue),
				static (state, it, grammars) =>
					new IsOneOfConstraint<TNumber>(it, grammars, state.ExpectedValues,
						state.DoNotPopulateThisValue),
				out IsOneOfConstraint<TNumber> constraint),
			subject,
			constraint);
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
		return new NullableNumberToleranceResult<TNumber, IThat<TNumber?>>(
			subject.Get().ExpectationBuilder.AddConstraint(
				(ExpectedValues: expectedValues, DoNotPopulateThisValue: doNotPopulateThisValue),
				static (state, it, grammars) =>
					new NullableIsOneOfConstraint<TNumber>(it, grammars, state.ExpectedValues,
						state.DoNotPopulateThisValue),
				out NullableIsOneOfConstraint<TNumber> constraint),
			subject,
			constraint);
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
		return new NumberToleranceResult<TNumber, IThat<TNumber>>(
			subject.Get().ExpectationBuilder.AddConstraint(
				(ExpectedValues: expectedValues, DoNotPopulateThisValue: doNotPopulateThisValue),
				static (state, it, grammars) =>
					new IsOneOfConstraintWithNullable<TNumber>(it, grammars, state.ExpectedValues,
						state.DoNotPopulateThisValue),
				out IsOneOfConstraintWithNullable<TNumber> constraint),
			subject,
			constraint);
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
		return new NullableNumberToleranceResult<TNumber, IThat<TNumber?>>(
			subject.Get().ExpectationBuilder.AddConstraint(
				(ExpectedValues: expectedValues, DoNotPopulateThisValue: doNotPopulateThisValue),
				static (state, it, grammars) =>
					new NullableIsOneOfConstraintWithNullable<TNumber>(it, grammars, state.ExpectedValues,
						state.DoNotPopulateThisValue),
				out NullableIsOneOfConstraintWithNullable<TNumber> constraint),
			subject,
			constraint);
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
		return new NumberToleranceResult<TNumber, IThat<TNumber>>(
			subject.Get().ExpectationBuilder.AddConstraint(unexpectedValues,
				static (unexpectedValues, it, grammars) =>
					new IsOneOfConstraintWithNullable<TNumber>(it, grammars, unexpectedValues,
						null).Invert(),
				out IsOneOfConstraintWithNullable<TNumber> constraint),
			subject,
			constraint);
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
		return new NullableNumberToleranceResult<TNumber, IThat<TNumber?>>(
			subject.Get().ExpectationBuilder.AddConstraint(unexpectedValues,
				static (unexpectedValues, it, grammars) =>
					new NullableIsOneOfConstraintWithNullable<TNumber>(it, grammars, unexpectedValues,
							null)
						.Invert(),
				out NullableIsOneOfConstraintWithNullable<TNumber> constraint),
			subject,
			constraint);
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
		return new NumberToleranceResult<TNumber, IThat<TNumber>>(
			subject.Get().ExpectationBuilder.AddConstraint(
				(UnexpectedValues: unexpectedValues, DoNotPopulateThisValue: doNotPopulateThisValue),
				static (state, it, grammars) =>
					new IsOneOfConstraint<TNumber>(it, grammars, state.UnexpectedValues,
							state.DoNotPopulateThisValue)
						.Invert(),
				out IsOneOfConstraint<TNumber> constraint),
			subject,
			constraint);
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
		return new NullableNumberToleranceResult<TNumber, IThat<TNumber?>>(
			subject.Get().ExpectationBuilder.AddConstraint(
				(UnexpectedValues: unexpectedValues, DoNotPopulateThisValue: doNotPopulateThisValue),
				static (state, it, grammars) =>
					new NullableIsOneOfConstraint<TNumber>(it, grammars, state.UnexpectedValues,
							state.DoNotPopulateThisValue)
						.Invert(),
				out NullableIsOneOfConstraint<TNumber> constraint),
			subject,
			constraint);
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
		return new NumberToleranceResult<TNumber, IThat<TNumber>>(
			subject.Get().ExpectationBuilder.AddConstraint(
				(UnexpectedValues: unexpectedValues, DoNotPopulateThisValue: doNotPopulateThisValue),
				static (state, it, grammars) =>
					new IsOneOfConstraintWithNullable<TNumber>(it, grammars, state.UnexpectedValues,
							state.DoNotPopulateThisValue)
						.Invert(),
				out IsOneOfConstraintWithNullable<TNumber> constraint),
			subject,
			constraint);
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
		return new NullableNumberToleranceResult<TNumber, IThat<TNumber?>>(
			subject.Get().ExpectationBuilder.AddConstraint(
				(UnexpectedValues: unexpectedValues, DoNotPopulateThisValue: doNotPopulateThisValue),
				static (state, it, grammars) =>
					new NullableIsOneOfConstraintWithNullable<TNumber>(it, grammars, state.UnexpectedValues,
							state.DoNotPopulateThisValue)
						.Invert(),
				out NullableIsOneOfConstraintWithNullable<TNumber> constraint),
			subject,
			constraint);
	}

	private sealed class IsOneOfConstraint<TNumber>(
		string it,
		ExpectationGrammars grammars,
		IEnumerable<TNumber> expected,
		string? expectedExpression)
		: ConstraintResult.WithValue<TNumber>(it, grammars),
			IValueConstraint<TNumber>,
			IOptionsHolderSource<NumberTolerance<TNumber>>
		where TNumber : struct, INumber<TNumber>
	{
		private OptionsHolder<NumberTolerance<TNumber>>? _optionsHolder;

		private NumberTolerance<TNumber> CurrentOptions
			=> _optionsHolder is null ? new NumberTolerance<TNumber>(CalculateDifference) : _optionsHolder.Value;

		/// <inheritdoc cref="IOptionsHolderSource{TOptions}.Holder" />
		OptionsHolder<NumberTolerance<TNumber>> IOptionsHolderSource<NumberTolerance<TNumber>>.Holder
			=> _optionsHolder ??= new OptionsHolder<NumberTolerance<TNumber>>(new NumberTolerance<TNumber>(CalculateDifference));

		public ConstraintResult IsMetBy(TNumber actual)
		{
			Actual = actual;
			Outcome = IsWithinToleranceOfAny(CurrentOptions, actual, expected);
			return this;
		}

		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
			=> contexts.AddExpectedValuesContext(expectedExpression, expected, Grammars);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is one of ", "are one of "));
			stringBuilder.Append(expectedExpression?.TrimCommonWhiteSpace() ?? Formatter.Format(expected));
			stringBuilder.Append(CurrentOptions);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
			AppendDifferenceToClosest(stringBuilder, Actual, expected.Select(value => (TNumber?)value), CurrentOptions);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not one of ", "are not one of "));
			stringBuilder.Append(expectedExpression?.TrimCommonWhiteSpace() ?? Formatter.Format(expected));
			stringBuilder.Append(CurrentOptions);
		}
	}

	private sealed class NullableIsOneOfConstraint<TNumber>(
		string it,
		ExpectationGrammars grammars,
		IEnumerable<TNumber> expected,
		string? expectedExpression)
		: ConstraintResult.WithValue<TNumber?>(it, grammars),
			IValueConstraint<TNumber?>,
			IOptionsHolderSource<NumberTolerance<TNumber>>
		where TNumber : struct, INumber<TNumber>
	{
		private OptionsHolder<NumberTolerance<TNumber>>? _optionsHolder;

		private NumberTolerance<TNumber> CurrentOptions
			=> _optionsHolder is null ? new NumberTolerance<TNumber>(CalculateDifference) : _optionsHolder.Value;

		/// <inheritdoc cref="IOptionsHolderSource{TOptions}.Holder" />
		OptionsHolder<NumberTolerance<TNumber>> IOptionsHolderSource<NumberTolerance<TNumber>>.Holder
			=> _optionsHolder ??= new OptionsHolder<NumberTolerance<TNumber>>(new NumberTolerance<TNumber>(CalculateDifference));

		public ConstraintResult IsMetBy(TNumber? actual)
		{
			Actual = actual;
			Outcome = IsWithinToleranceOfAny(CurrentOptions, actual, expected);
			return this;
		}

		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
			=> contexts.AddExpectedValuesContext(expectedExpression, expected, Grammars);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is one of ", "are one of "));
			stringBuilder.Append(expectedExpression?.TrimCommonWhiteSpace() ?? Formatter.Format(expected));
			stringBuilder.Append(CurrentOptions);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
			AppendDifferenceToClosest(stringBuilder, Actual, expected.Select(value => (TNumber?)value), CurrentOptions);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not one of ", "are not one of "));
			stringBuilder.Append(expectedExpression?.TrimCommonWhiteSpace() ?? Formatter.Format(expected));
			stringBuilder.Append(CurrentOptions);
		}
	}

	private sealed class IsOneOfConstraintWithNullable<TNumber>(
		string it,
		ExpectationGrammars grammars,
		IEnumerable<TNumber?> expected,
		string? expectedExpression)
		: ConstraintResult.WithValue<TNumber>(it, grammars),
			IValueConstraint<TNumber>,
			IOptionsHolderSource<NumberTolerance<TNumber>>
		where TNumber : struct, INumber<TNumber>
	{
		private OptionsHolder<NumberTolerance<TNumber>>? _optionsHolder;

		private NumberTolerance<TNumber> CurrentOptions
			=> _optionsHolder is null ? new NumberTolerance<TNumber>(CalculateDifference) : _optionsHolder.Value;

		/// <inheritdoc cref="IOptionsHolderSource{TOptions}.Holder" />
		OptionsHolder<NumberTolerance<TNumber>> IOptionsHolderSource<NumberTolerance<TNumber>>.Holder
			=> _optionsHolder ??= new OptionsHolder<NumberTolerance<TNumber>>(new NumberTolerance<TNumber>(CalculateDifference));

		public ConstraintResult IsMetBy(TNumber actual)
		{
			Actual = actual;
			Outcome = IsWithinToleranceOfAny(CurrentOptions, actual, expected);
			return this;
		}

		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
			=> contexts.AddExpectedValuesContext(expectedExpression, expected, Grammars);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is one of ", "are one of "));
			stringBuilder.Append(expectedExpression?.TrimCommonWhiteSpace() ?? Formatter.Format(expected));
			stringBuilder.Append(CurrentOptions);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
			AppendDifferenceToClosest(stringBuilder, Actual, expected, CurrentOptions);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not one of ", "are not one of "));
			stringBuilder.Append(expectedExpression?.TrimCommonWhiteSpace() ?? Formatter.Format(expected));
			stringBuilder.Append(CurrentOptions);
		}
	}

	private sealed class NullableIsOneOfConstraintWithNullable<TNumber>(
		string it,
		ExpectationGrammars grammars,
		IEnumerable<TNumber?> expected,
		string? expectedExpression)
		: ConstraintResult.WithValue<TNumber?>(it, grammars),
			IValueConstraint<TNumber?>,
			IOptionsHolderSource<NumberTolerance<TNumber>>
		where TNumber : struct, INumber<TNumber>
	{
		private OptionsHolder<NumberTolerance<TNumber>>? _optionsHolder;

		private NumberTolerance<TNumber> CurrentOptions
			=> _optionsHolder is null ? new NumberTolerance<TNumber>(CalculateDifference) : _optionsHolder.Value;

		/// <inheritdoc cref="IOptionsHolderSource{TOptions}.Holder" />
		OptionsHolder<NumberTolerance<TNumber>> IOptionsHolderSource<NumberTolerance<TNumber>>.Holder
			=> _optionsHolder ??= new OptionsHolder<NumberTolerance<TNumber>>(new NumberTolerance<TNumber>(CalculateDifference));

		public ConstraintResult IsMetBy(TNumber? actual)
		{
			Actual = actual;
			Outcome = IsWithinToleranceOfAny(CurrentOptions, actual, expected);
			return this;
		}

		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
			=> contexts.AddExpectedValuesContext(expectedExpression, expected, Grammars);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is one of ", "are one of "));
			stringBuilder.Append(expectedExpression?.TrimCommonWhiteSpace() ?? Formatter.Format(expected));
			stringBuilder.Append(CurrentOptions);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
			AppendDifferenceToClosest(stringBuilder, Actual, expected, CurrentOptions);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not one of ", "are not one of "));
			stringBuilder.Append(expectedExpression?.TrimCommonWhiteSpace() ?? Formatter.Format(expected));
			stringBuilder.Append(CurrentOptions);
		}
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
		NumberTolerance<TNumber> initialOptions,
		string? expectedExpression,
		bool negated)
		where TNumber : struct, IComparable<TNumber>
	{
		NumberTolerance<TNumber> options = initialOptions;
		IEnumerable<TNumber?> expectedValues = expected.ToNonEmptyValues(negated);
		return new NumberToleranceResult<TNumber, IThat<TNumber>>(
			subject.Get().ExpectationBuilder.AddConstraint(
				(ExpectedValues: expectedValues, ExpectedExpression: expectedExpression, Options: options,
					Negated: negated),
				static (state, it, grammars) =>
					new IsOneOfConstraintWithNullable<TNumber>(it, grammars, state.ExpectedValues,
						state.ExpectedExpression, state.Options)
						.InvertIf(state.Negated),
				out IsOneOfConstraintWithNullable<TNumber> constraint),
			subject,
			constraint);
	}

	[CreateExpectationFamily("Is{Not}OneOf", Factory = typeof(NumberToleranceFactory),
		Summary = IsOneOfSummary, NegatedSummary = IsNotOneOfSummary)]
	internal static NumberToleranceResult<TNumber, IThat<TNumber>> IsOneOfCore<TNumber>(
		IThat<TNumber> subject,
		IEnumerable<TNumber> expected,
		NumberTolerance<TNumber> initialOptions,
		string? expectedExpression,
		bool negated)
		where TNumber : struct, IComparable<TNumber>
	{
		NumberTolerance<TNumber> options = initialOptions;
		IEnumerable<TNumber> expectedValues = expected.ToNonEmptyValues(negated);
		return new NumberToleranceResult<TNumber, IThat<TNumber>>(
			subject.Get().ExpectationBuilder.AddConstraint(
				(ExpectedValues: expectedValues, ExpectedExpression: expectedExpression, Options: options,
					Negated: negated),
				static (state, it, grammars) =>
					new IsOneOfConstraint<TNumber>(it, grammars, state.ExpectedValues,
						state.ExpectedExpression, state.Options)
						.InvertIf(state.Negated),
				out IsOneOfConstraint<TNumber> constraint),
			subject,
			constraint);
	}

	[CreateExpectationFamily("Is{Not}OneOf", Factory = typeof(NumberToleranceFactory),
		Summary = IsOneOfSummary, NegatedSummary = IsNotOneOfSummary)]
	[CreateExpectationFamily("Is{Not}OneOf", Factory = typeof(NumberToleranceFactory), Params = true,
		Summary = IsOneOfSummary, NegatedSummary = IsNotOneOfSummary)]
	internal static NullableNumberToleranceResult<TNumber, IThat<TNumber?>>
		IsOneOfForNullableWithNullableValuesCore<TNumber>(
			IThat<TNumber?> subject,
			IEnumerable<TNumber?> expected,
			NumberTolerance<TNumber> initialOptions,
			string? expectedExpression,
			bool negated)
		where TNumber : struct, IComparable<TNumber>
	{
		NumberTolerance<TNumber> options = initialOptions;
		IEnumerable<TNumber?> expectedValues = expected.ToNonEmptyValues(negated);
		return new NullableNumberToleranceResult<TNumber, IThat<TNumber?>>(
			subject.Get().ExpectationBuilder.AddConstraint(
				(ExpectedValues: expectedValues, ExpectedExpression: expectedExpression, Options: options,
					Negated: negated),
				static (state, it, grammars) =>
					new NullableIsOneOfConstraintWithNullable<TNumber>(it, grammars, state.ExpectedValues,
						state.ExpectedExpression, state.Options)
						.InvertIf(state.Negated),
				out NullableIsOneOfConstraintWithNullable<TNumber> constraint),
			subject,
			constraint);
	}

	[CreateExpectationFamily("Is{Not}OneOf", Factory = typeof(NumberToleranceFactory),
		Summary = IsOneOfSummary, NegatedSummary = IsNotOneOfSummary)]
	internal static NullableNumberToleranceResult<TNumber, IThat<TNumber?>> IsOneOfForNullableCore<TNumber>(
		IThat<TNumber?> subject,
		IEnumerable<TNumber> expected,
		NumberTolerance<TNumber> initialOptions,
		string? expectedExpression,
		bool negated)
		where TNumber : struct, IComparable<TNumber>
	{
		NumberTolerance<TNumber> options = initialOptions;
		IEnumerable<TNumber> expectedValues = expected.ToNonEmptyValues(negated);
		return new NullableNumberToleranceResult<TNumber, IThat<TNumber?>>(
			subject.Get().ExpectationBuilder.AddConstraint(
				(ExpectedValues: expectedValues, ExpectedExpression: expectedExpression, Options: options,
					Negated: negated),
				static (state, it, grammars) =>
					new NullableIsOneOfConstraint<TNumber>(it, grammars, state.ExpectedValues,
						state.ExpectedExpression, state.Options)
						.InvertIf(state.Negated),
				out NullableIsOneOfConstraint<TNumber> constraint),
			subject,
			constraint);
	}

	private sealed class IsOneOfConstraint<TNumber>(
		string it,
		ExpectationGrammars grammars,
		IEnumerable<TNumber> expected,
		string? expectedExpression,
		NumberTolerance<TNumber> options)
		: ConstraintResult.WithValue<TNumber>(it, grammars),
			IValueConstraint<TNumber>,
			IOptionsHolderSource<NumberTolerance<TNumber>>
		where TNumber : struct, IComparable<TNumber>
	{
		private OptionsHolder<NumberTolerance<TNumber>>? _optionsHolder;

		private NumberTolerance<TNumber> CurrentOptions => _optionsHolder is null ? options : _optionsHolder.Value;

		/// <inheritdoc cref="IOptionsHolderSource{TOptions}.Holder" />
		OptionsHolder<NumberTolerance<TNumber>> IOptionsHolderSource<NumberTolerance<TNumber>>.Holder
			=> _optionsHolder ??= new OptionsHolder<NumberTolerance<TNumber>>(options);

		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
			=> contexts.AddExpectedValuesContext(expectedExpression, expected, Grammars);

		public ConstraintResult IsMetBy(TNumber actual)
		{
			Actual = actual;
			Outcome = IsWithinToleranceOfAny(CurrentOptions, actual, expected);
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is one of ", "are one of "));
			stringBuilder.Append(expectedExpression?.TrimCommonWhiteSpace()
			                     ?? ValueFormatters.Format(Formatter, expected));
			stringBuilder.Append(CurrentOptions);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
			AppendDifferenceToClosest(stringBuilder, Actual, expected.Select(value => (TNumber?)value), CurrentOptions);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not one of ", "are not one of "));
			stringBuilder.Append(expectedExpression?.TrimCommonWhiteSpace()
			                     ?? ValueFormatters.Format(Formatter, expected));
			stringBuilder.Append(CurrentOptions);
		}
	}

	private sealed class NullableIsOneOfConstraint<TNumber>(
		string it,
		ExpectationGrammars grammars,
		IEnumerable<TNumber> expected,
		string? expectedExpression,
		NumberTolerance<TNumber> options)
		: ConstraintResult.WithValue<TNumber?>(it, grammars),
			IValueConstraint<TNumber?>,
			IOptionsHolderSource<NumberTolerance<TNumber>>
		where TNumber : struct, IComparable<TNumber>
	{
		private OptionsHolder<NumberTolerance<TNumber>>? _optionsHolder;

		private NumberTolerance<TNumber> CurrentOptions => _optionsHolder is null ? options : _optionsHolder.Value;

		/// <inheritdoc cref="IOptionsHolderSource{TOptions}.Holder" />
		OptionsHolder<NumberTolerance<TNumber>> IOptionsHolderSource<NumberTolerance<TNumber>>.Holder
			=> _optionsHolder ??= new OptionsHolder<NumberTolerance<TNumber>>(options);

		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
			=> contexts.AddExpectedValuesContext(expectedExpression, expected, Grammars);

		public ConstraintResult IsMetBy(TNumber? actual)
		{
			Actual = actual;
			Outcome = IsWithinToleranceOfAny(CurrentOptions, actual, expected);
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is one of ", "are one of "));
			stringBuilder.Append(expectedExpression?.TrimCommonWhiteSpace()
			                     ?? ValueFormatters.Format(Formatter, expected));
			stringBuilder.Append(CurrentOptions);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
			AppendDifferenceToClosest(stringBuilder, Actual, expected.Select(value => (TNumber?)value), CurrentOptions);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not one of ", "are not one of "));
			stringBuilder.Append(expectedExpression?.TrimCommonWhiteSpace()
			                     ?? ValueFormatters.Format(Formatter, expected));
			stringBuilder.Append(CurrentOptions);
		}
	}

	private sealed class IsOneOfConstraintWithNullable<TNumber>(
		string it,
		ExpectationGrammars grammars,
		IEnumerable<TNumber?> expected,
		string? expectedExpression,
		NumberTolerance<TNumber> options)
		: ConstraintResult.WithValue<TNumber>(it, grammars),
			IValueConstraint<TNumber>,
			IOptionsHolderSource<NumberTolerance<TNumber>>
		where TNumber : struct, IComparable<TNumber>
	{
		private OptionsHolder<NumberTolerance<TNumber>>? _optionsHolder;

		private NumberTolerance<TNumber> CurrentOptions => _optionsHolder is null ? options : _optionsHolder.Value;

		/// <inheritdoc cref="IOptionsHolderSource{TOptions}.Holder" />
		OptionsHolder<NumberTolerance<TNumber>> IOptionsHolderSource<NumberTolerance<TNumber>>.Holder
			=> _optionsHolder ??= new OptionsHolder<NumberTolerance<TNumber>>(options);

		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
			=> contexts.AddExpectedValuesContext(expectedExpression, expected, Grammars);

		public ConstraintResult IsMetBy(TNumber actual)
		{
			Actual = actual;
			Outcome = IsWithinToleranceOfAny(CurrentOptions, actual, expected);
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is one of ", "are one of "));
			stringBuilder.Append(expectedExpression?.TrimCommonWhiteSpace()
			                     ?? ValueFormatters.Format(Formatter, expected));
			stringBuilder.Append(CurrentOptions);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
			AppendDifferenceToClosest(stringBuilder, Actual, expected, CurrentOptions);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not one of ", "are not one of "));
			stringBuilder.Append(expectedExpression?.TrimCommonWhiteSpace()
			                     ?? ValueFormatters.Format(Formatter, expected));
			stringBuilder.Append(CurrentOptions);
		}
	}

	private sealed class NullableIsOneOfConstraintWithNullable<TNumber>(
		string it,
		ExpectationGrammars grammars,
		IEnumerable<TNumber?> expected,
		string? expectedExpression,
		NumberTolerance<TNumber> options)
		: ConstraintResult.WithValue<TNumber?>(it, grammars),
			IValueConstraint<TNumber?>,
			IOptionsHolderSource<NumberTolerance<TNumber>>
		where TNumber : struct, IComparable<TNumber>
	{
		private OptionsHolder<NumberTolerance<TNumber>>? _optionsHolder;

		private NumberTolerance<TNumber> CurrentOptions => _optionsHolder is null ? options : _optionsHolder.Value;

		/// <inheritdoc cref="IOptionsHolderSource{TOptions}.Holder" />
		OptionsHolder<NumberTolerance<TNumber>> IOptionsHolderSource<NumberTolerance<TNumber>>.Holder
			=> _optionsHolder ??= new OptionsHolder<NumberTolerance<TNumber>>(options);

		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
			=> contexts.AddExpectedValuesContext(expectedExpression, expected, Grammars);

		public ConstraintResult IsMetBy(TNumber? actual)
		{
			Actual = actual;
			Outcome = IsWithinToleranceOfAny(CurrentOptions, actual, expected);
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is one of ", "are one of "));
			stringBuilder.Append(expectedExpression?.TrimCommonWhiteSpace()
			                     ?? ValueFormatters.Format(Formatter, expected));
			stringBuilder.Append(CurrentOptions);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
			AppendDifferenceToClosest(stringBuilder, Actual, expected, CurrentOptions);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not one of ", "are not one of "));
			stringBuilder.Append(expectedExpression?.TrimCommonWhiteSpace()
			                     ?? ValueFormatters.Format(Formatter, expected));
			stringBuilder.Append(CurrentOptions);
		}
	}
#endif

	/// <remarks>
	///     The options are read once and the candidates are checked without a closure, because this runs for every
	///     item when the expectation is applied to the items of a collection. An array, e.g. from the
	///     <see langword="params" /> overloads, is iterated directly to avoid allocating an enumerator.
	/// </remarks>
	private static Outcome IsWithinToleranceOfAny<TNumber>(NumberTolerance<TNumber> options, TNumber? actual,
		IEnumerable<TNumber> expected)
#if NET8_0_OR_GREATER
		where TNumber : struct, INumber<TNumber>
#else
		where TNumber : struct, IComparable<TNumber>
#endif
	{
		if (expected is TNumber[] array)
		{
			// A foreach over the array type compiles to an indexed loop, while one over IEnumerable<T> allocates an enumerator.
			foreach (TNumber value in array)
			{
				if (options.IsWithinTolerance(actual, value))
				{
					return Outcome.Success;
				}
			}

			return Outcome.Failure;
		}

		foreach (TNumber value in expected)
		{
			if (options.IsWithinTolerance(actual, value))
			{
				return Outcome.Success;
			}
		}

		return Outcome.Failure;
	}

	/// <inheritdoc cref="IsWithinToleranceOfAny{TNumber}(NumberTolerance{TNumber}, TNumber?, IEnumerable{TNumber})" />
	private static Outcome IsWithinToleranceOfAny<TNumber>(NumberTolerance<TNumber> options, TNumber? actual,
		IEnumerable<TNumber?> expected)
#if NET8_0_OR_GREATER
		where TNumber : struct, INumber<TNumber>
#else
		where TNumber : struct, IComparable<TNumber>
#endif
	{
		if (expected is TNumber?[] array)
		{
			// A foreach over the array type compiles to an indexed loop, while one over IEnumerable<T> allocates an enumerator.
			foreach (TNumber? value in array)
			{
				if (options.IsWithinTolerance(actual, value))
				{
					return Outcome.Success;
				}
			}

			return Outcome.Failure;
		}

		foreach (TNumber? value in expected)
		{
			if (options.IsWithinTolerance(actual, value))
			{
				return Outcome.Success;
			}
		}

		return Outcome.Failure;
	}
}
