using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
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
	///     Verifies that the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	public static NumberToleranceResult<TNumber, IThat<TNumber>> IsEqualTo<TNumber>(
		this IThat<TNumber> subject, TNumber? expected)
		where TNumber : struct, INumber<TNumber>
	{
		return new NumberToleranceResult<TNumber, IThat<TNumber>>(
			subject.Get().ExpectationBuilder.AddConstraint(expected,
				static (expected, it, grammars) =>
					new IsEqualToConstraint<TNumber>(it, grammars, expected),
				out IsEqualToConstraint<TNumber> constraint),
			subject,
			constraint);
	}

	/// <summary>
	///     Verifies that the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	public static NullableNumberToleranceResult<TNumber, IThat<TNumber?>> IsEqualTo<TNumber>(
		this IThat<TNumber?> subject, TNumber? expected)
		where TNumber : struct, INumber<TNumber>
	{
		return new NullableNumberToleranceResult<TNumber, IThat<TNumber?>>(
			subject.Get().ExpectationBuilder.AddConstraint(expected,
				static (expected, it, grammars) =>
					new NullableIsEqualToConstraint<TNumber>(it, grammars, expected),
				out NullableIsEqualToConstraint<TNumber> constraint),
			subject,
			constraint);
	}

	/// <summary>
	///     Verifies that the subject is not equal to the <paramref name="unexpected" /> value.
	/// </summary>
	public static NumberToleranceResult<TNumber, IThat<TNumber>> IsNotEqualTo<TNumber>(
		this IThat<TNumber> subject, TNumber? unexpected)
		where TNumber : struct, INumber<TNumber>
	{
		return new NumberToleranceResult<TNumber, IThat<TNumber>>(
			subject.Get().ExpectationBuilder.AddConstraint(unexpected,
				static (unexpected, it, grammars) =>
					new IsEqualToConstraint<TNumber>(it, grammars, unexpected).Invert(),
				out IsEqualToConstraint<TNumber> constraint),
			subject,
			constraint);
	}

	/// <summary>
	///     Verifies that the subject is not equal to the <paramref name="unexpected" /> value.
	/// </summary>
	public static NullableNumberToleranceResult<TNumber, IThat<TNumber?>> IsNotEqualTo<TNumber>(
		this IThat<TNumber?> subject, TNumber? unexpected)
		where TNumber : struct, INumber<TNumber>
	{
		return new NullableNumberToleranceResult<TNumber, IThat<TNumber?>>(
			subject.Get().ExpectationBuilder.AddConstraint(unexpected,
				static (unexpected, it, grammars) =>
					new NullableIsEqualToConstraint<TNumber>(it, grammars, unexpected).Invert(),
				out NullableIsEqualToConstraint<TNumber> constraint),
			subject,
			constraint);
	}

	private sealed class IsEqualToConstraint<TNumber>(
		string it,
		ExpectationGrammars grammars,
		TNumber? expected)
		: ConstraintResult.WithEqualToValue<TNumber>(it, grammars, expected is null),
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
			Outcome = CurrentOptions.IsWithinTolerance(actual, expected)
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is equal to ", "are equal to "));
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(CurrentOptions);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
			AppendDifference(stringBuilder, Actual, expected);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not equal to ", "are not equal to "));
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(CurrentOptions);
		}
	}

	private sealed class NullableIsEqualToConstraint<TNumber>(
		string it,
		ExpectationGrammars grammars,
		TNumber? expected)
		: ConstraintResult.WithEqualToValue<TNumber?>(it, grammars, expected is null),
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
			Outcome = CurrentOptions.IsWithinTolerance(actual, expected)
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is equal to ", "are equal to "));
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(CurrentOptions);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
			AppendDifference(stringBuilder, Actual, expected);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not equal to ", "are not equal to "));
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(CurrentOptions);
		}
	}
#else
	private const string IsEqualToSummary =
		"Verifies that the subject is equal to the <paramref name=\"expected\" /> value.";

	private const string IsNotEqualToSummary =
		"Verifies that the subject is not equal to the <paramref name=\"unexpected\" /> value.";

	[CreateExpectationFamily("Is{Not}EqualTo", Factory = typeof(NumberToleranceFactory),
		Summary = IsEqualToSummary, NegatedSummary = IsNotEqualToSummary)]
	internal static NumberToleranceResult<TNumber, IThat<TNumber>> IsEqualToCore<TNumber>(
		IThat<TNumber> subject,
		TNumber? expected,
		NumberTolerance<TNumber> initialOptions,
		bool negated)
		where TNumber : struct, IComparable<TNumber>
	{
		NumberTolerance<TNumber> options = initialOptions;
		return new(subject.Get().ExpectationBuilder.AddConstraint((Expected: expected, Options: options, Negated: negated),
				static (state, it, grammars) =>
					new IsEqualToConstraint<TNumber>(it, grammars, state.Expected, state.Options)
						.InvertIf(state.Negated),
			out IsEqualToConstraint<TNumber> constraint),
			subject,
			constraint);
	}

	[CreateExpectationFamily("Is{Not}EqualTo", Factory = typeof(NumberToleranceFactory),
		Summary = IsEqualToSummary, NegatedSummary = IsNotEqualToSummary)]
	internal static NullableNumberToleranceResult<TNumber, IThat<TNumber?>> IsEqualToForNullableCore<TNumber>(
		IThat<TNumber?> subject,
		TNumber? expected,
		NumberTolerance<TNumber> initialOptions,
		bool negated)
		where TNumber : struct, IComparable<TNumber>
	{
		NumberTolerance<TNumber> options = initialOptions;
		return new(subject.Get().ExpectationBuilder.AddConstraint((Expected: expected, Options: options, Negated: negated),
				static (state, it, grammars) =>
					new NullableIsEqualToConstraint<TNumber>(it, grammars, state.Expected, state.Options)
						.InvertIf(state.Negated),
			out NullableIsEqualToConstraint<TNumber> constraint),
			subject,
			constraint);
	}

	private sealed class IsEqualToConstraint<TNumber>(
		string it,
		ExpectationGrammars grammars,
		TNumber? expected,
		NumberTolerance<TNumber> options)
		: ConstraintResult.WithEqualToValue<TNumber>(it, grammars, expected is null),
			IValueConstraint<TNumber>,
			IOptionsHolderSource<NumberTolerance<TNumber>>
		where TNumber : struct, IComparable<TNumber>
	{
		private OptionsHolder<NumberTolerance<TNumber>>? _optionsHolder;

		private NumberTolerance<TNumber> CurrentOptions => _optionsHolder is null ? options : _optionsHolder.Value;

		/// <inheritdoc cref="IOptionsHolderSource{TOptions}.Holder" />
		OptionsHolder<NumberTolerance<TNumber>> IOptionsHolderSource<NumberTolerance<TNumber>>.Holder
			=> _optionsHolder ??= new OptionsHolder<NumberTolerance<TNumber>>(options);

		public ConstraintResult IsMetBy(TNumber actual)
		{
			Actual = actual;
			Outcome = CurrentOptions.IsWithinTolerance(actual, expected) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is equal to ", "are equal to "));
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(CurrentOptions);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
			AppendDifference(stringBuilder, Actual, expected, CurrentOptions);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not equal to ", "are not equal to "));
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(CurrentOptions);
		}
	}

	private sealed class NullableIsEqualToConstraint<TNumber>(
		string it,
		ExpectationGrammars grammars,
		TNumber? expected,
		NumberTolerance<TNumber> options)
		: ConstraintResult.WithEqualToValue<TNumber?>(it, grammars, expected is null),
			IValueConstraint<TNumber?>,
			IOptionsHolderSource<NumberTolerance<TNumber>>
		where TNumber : struct, IComparable<TNumber>
	{
		private OptionsHolder<NumberTolerance<TNumber>>? _optionsHolder;

		private NumberTolerance<TNumber> CurrentOptions => _optionsHolder is null ? options : _optionsHolder.Value;

		/// <inheritdoc cref="IOptionsHolderSource{TOptions}.Holder" />
		OptionsHolder<NumberTolerance<TNumber>> IOptionsHolderSource<NumberTolerance<TNumber>>.Holder
			=> _optionsHolder ??= new OptionsHolder<NumberTolerance<TNumber>>(options);

		public ConstraintResult IsMetBy(TNumber? actual)
		{
			Actual = actual;
			Outcome = CurrentOptions.IsWithinTolerance(actual, expected) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is equal to ", "are equal to "));
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(CurrentOptions);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
			AppendDifference(stringBuilder, Actual, expected, CurrentOptions);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not equal to ", "are not equal to "));
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(CurrentOptions);
		}
	}
#endif
}
