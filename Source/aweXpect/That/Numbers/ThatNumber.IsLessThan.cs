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
	///     Verifies that the subject is less than the <paramref name="expected" /> value.
	/// </summary>
	/// <remarks>
	///     A <c>NaN</c> <paramref name="expected" /> value throws an <see cref="System.ArgumentOutOfRangeException" />.
	/// </remarks>
	public static NumberToleranceResult<TNumber, IThat<TNumber>> IsLessThan<TNumber>(
		this IThat<TNumber> subject, TNumber? expected)
		where TNumber : struct, INumber<TNumber>
	{
		expected.ThrowIfNaN("expected value");
		return new NumberToleranceResult<TNumber, IThat<TNumber>>(
			subject.Get().ExpectationBuilder.AddConstraint(expected,
				static (expected, it, grammars) =>
					new IsLessThanConstraint<TNumber>(it, grammars, expected),
				out IsLessThanConstraint<TNumber> constraint),
			subject,
			constraint);
	}

	/// <summary>
	///     Verifies that the subject is less than the <paramref name="expected" /> value.
	/// </summary>
	/// <remarks>
	///     A <c>NaN</c> <paramref name="expected" /> value throws an <see cref="System.ArgumentOutOfRangeException" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static NumberToleranceResult<TNumber, IThat<TNumber?>> IsLessThan<TNumber>(
		this IThat<TNumber?> subject, TNumber? expected)
		where TNumber : struct, INumber<TNumber>
	{
		expected.ThrowIfNaN("expected value");
		return new NumberToleranceResult<TNumber, IThat<TNumber?>>(
			subject.Get().ExpectationBuilder.AddConstraint(expected,
				static (expected, it, grammars) =>
					new NullableIsLessThanConstraint<TNumber>(it, grammars, expected),
				out NullableIsLessThanConstraint<TNumber> constraint),
			subject,
			constraint);
	}

	/// <summary>
	///     Verifies that the subject is not less than the <paramref name="unexpected" /> value.
	/// </summary>
	/// <remarks>
	///     A <c>NaN</c> subject is not less than any value, so it satisfies this expectation
	///     although it fails <c>IsGreaterThanOrEqualTo</c>. A <c>NaN</c> <paramref name="unexpected" /> value throws an
	///     <see cref="System.ArgumentOutOfRangeException" />, while a <see langword="null" /> one fails this
	///     expectation as well as <c>IsLessThan</c>.
	/// </remarks>
	public static NumberToleranceResult<TNumber, IThat<TNumber>> IsNotLessThan<TNumber>(
		this IThat<TNumber> subject, TNumber? unexpected)
		where TNumber : struct, INumber<TNumber>
	{
		unexpected.ThrowIfNaN("unexpected value");
		return new NumberToleranceResult<TNumber, IThat<TNumber>>(
			subject.Get().ExpectationBuilder.AddConstraint(unexpected,
				static (unexpected, it, grammars) =>
					new IsLessThanConstraint<TNumber>(it, grammars, unexpected).Invert(),
				out IsLessThanConstraint<TNumber> constraint),
			subject,
			constraint);
	}

	/// <summary>
	///     Verifies that the subject is not less than the <paramref name="unexpected" /> value.
	/// </summary>
	/// <remarks>
	///     A <c>NaN</c> subject is not less than any value, so it satisfies this expectation
	///     although it fails <c>IsGreaterThanOrEqualTo</c>. A <c>NaN</c> <paramref name="unexpected" /> value throws an
	///     <see cref="System.ArgumentOutOfRangeException" />, while a <see langword="null" /> one fails this
	///     expectation as well as <c>IsLessThan</c>.
	/// </remarks>
	[GuaranteesNotNull]
	public static NumberToleranceResult<TNumber, IThat<TNumber?>> IsNotLessThan<TNumber>(
		this IThat<TNumber?> subject, TNumber? unexpected)
		where TNumber : struct, INumber<TNumber>
	{
		unexpected.ThrowIfNaN("unexpected value");
		return new NumberToleranceResult<TNumber, IThat<TNumber?>>(
			subject.Get().ExpectationBuilder.AddConstraint(unexpected,
				static (unexpected, it, grammars) =>
					new NullableIsLessThanConstraint<TNumber>(it, grammars, unexpected).Invert(),
				out NullableIsLessThanConstraint<TNumber> constraint),
			subject,
			constraint);
	}

	private sealed class IsLessThanConstraint<TNumber>(
		string it,
		ExpectationGrammars grammars,
		TNumber? expected)
		: OrderingConstraint<TNumber>(it, grammars, expected is null),
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
			if (IsIncomparable)
			{
				Outcome = Outcome.FailureBothWays;
				return this;
			}

			Outcome = CurrentOptions.IsLessThan(actual, expected) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is less than ", "are less than "));
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
			stringBuilder.Append(Grammars.Verb("is not less than ", "are not less than "));
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(CurrentOptions);
		}
	}

	private sealed class NullableIsLessThanConstraint<TNumber>(
		string it,
		ExpectationGrammars grammars,
		TNumber? expected)
		: OrderingConstraint<TNumber?>(it, grammars, expected is null),
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
			if (IsIncomparable)
			{
				Outcome = Outcome.FailureBothWays;
				return this;
			}

			Outcome = CurrentOptions.IsLessThan(actual, expected) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is less than ", "are less than "));
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
			stringBuilder.Append(Grammars.Verb("is not less than ", "are not less than "));
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(CurrentOptions);
		}
	}
#else
	private const string IsLessThanSummary =
		"Verifies that the subject is less than the <paramref name=\"expected\" /> value.";

	private const string IsLessThanRemarks =
		"A <c>NaN</c> <paramref name=\"expected\" /> value throws an <see cref=\"System.ArgumentOutOfRangeException\" />.";

	private const string IsNotLessThanSummary =
		"Verifies that the subject is not less than the <paramref name=\"unexpected\" /> value.";

	private const string IsNotLessThanRemarks =
		"A <c>NaN</c> subject is not less than any value, so it satisfies this expectation\n" +
		"although it fails <c>IsGreaterThanOrEqualTo</c>. A <c>NaN</c> <paramref name=\"unexpected\" /> value throws an\n" +
		"<see cref=\"System.ArgumentOutOfRangeException\" />, while a <see langword=\"null\" /> one fails this\n" +
		"expectation as well as <c>IsLessThan</c>.";

	[CreateExpectationFamily("Is{Not}LessThan", Factory = typeof(NumberToleranceFactory),
		Summary = IsLessThanSummary, NegatedSummary = IsNotLessThanSummary,
		Remarks = IsLessThanRemarks, NegatedRemarks = IsNotLessThanRemarks)]
	internal static NumberToleranceResult<TNumber, IThat<TNumber>> IsLessThanCore<TNumber>(
		IThat<TNumber> subject,
		TNumber? expected,
		NumberTolerance<TNumber> initialOptions,
		bool negated)
		where TNumber : struct, IComparable<TNumber>
	{
		NumberTolerance<TNumber> options = initialOptions;
		ThrowIfNaN(expected, negated);
		return new NumberToleranceResult<TNumber, IThat<TNumber>>(
			subject.Get().ExpectationBuilder.AddConstraint((Expected: expected, Options: options, Negated: negated),
				static (state, it, grammars) =>
					new IsLessThanConstraint<TNumber>(it, grammars, state.Expected, state.Options)
						.InvertIf(state.Negated),
				out IsLessThanConstraint<TNumber> constraint),
			subject,
			constraint);
	}

	[CreateExpectationFamily("Is{Not}LessThan", Factory = typeof(NumberToleranceFactory), GuaranteesNotNull = true,
		Summary = IsLessThanSummary, NegatedSummary = IsNotLessThanSummary,
		Remarks = IsLessThanRemarks, NegatedRemarks = IsNotLessThanRemarks)]
	internal static NumberToleranceResult<TNumber, IThat<TNumber?>> IsLessThanForNullableCore<TNumber>(
		IThat<TNumber?> subject,
		TNumber? expected,
		NumberTolerance<TNumber> initialOptions,
		bool negated)
		where TNumber : struct, IComparable<TNumber>
	{
		NumberTolerance<TNumber> options = initialOptions;
		ThrowIfNaN(expected, negated);
		return new NumberToleranceResult<TNumber, IThat<TNumber?>>(
			subject.Get().ExpectationBuilder.AddConstraint((Expected: expected, Options: options, Negated: negated),
				static (state, it, grammars) =>
					new NullableIsLessThanConstraint<TNumber>(it, grammars, state.Expected, state.Options)
						.InvertIf(state.Negated),
				out NullableIsLessThanConstraint<TNumber> constraint),
			subject,
			constraint);
	}

	private sealed class IsLessThanConstraint<TNumber>(
		string it,
		ExpectationGrammars grammars,
		TNumber? expected,
		NumberTolerance<TNumber> options)
		: OrderingConstraint<TNumber>(it, grammars, expected is null),
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
			if (IsIncomparable)
			{
				Outcome = Outcome.FailureBothWays;
				return this;
			}

			Outcome = CurrentOptions.IsLessThan(actual, expected) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is less than ", "are less than "));
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
			stringBuilder.Append(Grammars.Verb("is not less than ", "are not less than "));
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(CurrentOptions);
		}
	}

	private sealed class NullableIsLessThanConstraint<TNumber>(
		string it,
		ExpectationGrammars grammars,
		TNumber? expected,
		NumberTolerance<TNumber> options)
		: OrderingConstraint<TNumber?>(it, grammars, expected is null),
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
			if (IsIncomparable)
			{
				Outcome = Outcome.FailureBothWays;
				return this;
			}

			Outcome = CurrentOptions.IsLessThan(actual, expected) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is less than ", "are less than "));
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
			stringBuilder.Append(Grammars.Verb("is not less than ", "are not less than "));
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(CurrentOptions);
		}
	}
#endif
}
