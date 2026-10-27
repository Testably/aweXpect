using System;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;
#if !NET8_0_OR_GREATER
using aweXpect.SourceGenerators;
#endif

namespace aweXpect;

public static partial class ThatNumber
{
#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the subject is between the <paramref name="minimum" />…
	/// </summary>
	/// <remarks>
	///     Both bounds are inclusive. A maximum below the <paramref name="minimum" /> or a <c>NaN</c> bound throws an
	///     <see cref="ArgumentOutOfRangeException" />, while a <see langword="null" /> bound fails the expectation as
	///     well as its negation. A <c>NaN</c> subject is never between them.
	/// </remarks>
	public static BetweenResult<NumberToleranceResult<TNumber, IThat<TNumber>>, TNumber?> IsBetween<TNumber>(
		this IThat<TNumber> subject, TNumber? minimum)
		where TNumber : struct, INumber<TNumber>
		=> new(maximum =>
		{
			return new NumberToleranceResult<TNumber, IThat<TNumber>>(
				subject.Get().ExpectationBuilder.AddConstraint(
					(Minimum: minimum, Maximum: maximum),
					static (state, it, grammars) =>
						new IsBetweenConstraint<TNumber>(it, grammars, state.Minimum, state.Maximum),
					out IsBetweenConstraint<TNumber> constraint),
				subject,
				constraint);
		});

	/// <summary>
	///     Verifies that the subject is between the <paramref name="minimum" />…
	/// </summary>
	/// <remarks>
	///     Both bounds are inclusive. A maximum below the <paramref name="minimum" /> or a <c>NaN</c> bound throws an
	///     <see cref="ArgumentOutOfRangeException" />, while a <see langword="null" /> bound fails the expectation as
	///     well as its negation. A <c>NaN</c> subject is never between them.
	/// </remarks>
	[GuaranteesNotNull]
	public static BetweenResult<NumberToleranceResult<TNumber, IThat<TNumber?>>, TNumber?> IsBetween<TNumber>(
		this IThat<TNumber?> subject, TNumber? minimum)
		where TNumber : struct, INumber<TNumber>
		=> new(maximum =>
		{
			return new NumberToleranceResult<TNumber, IThat<TNumber?>>(
				subject.Get().ExpectationBuilder.AddConstraint(
					(Minimum: minimum, Maximum: maximum),
					static (state, it, grammars) =>
						new NullableIsBetweenConstraint<TNumber>(it, grammars, state.Minimum, state.Maximum),
					out NullableIsBetweenConstraint<TNumber> constraint),
				subject,
				constraint);
		});

	/// <summary>
	///     Verifies that the subject is not between the <paramref name="minimum" />…
	/// </summary>
	/// <remarks>
	///     Both bounds are inclusive. A maximum below the <paramref name="minimum" /> or a <c>NaN</c> bound throws an
	///     <see cref="ArgumentOutOfRangeException" />, while a <see langword="null" /> bound fails the expectation as
	///     well as its negation. A <c>NaN</c> subject is never between them.
	/// </remarks>
	public static BetweenResult<NumberToleranceResult<TNumber, IThat<TNumber>>, TNumber?> IsNotBetween<TNumber>(
		this IThat<TNumber> subject, TNumber? minimum)
		where TNumber : struct, INumber<TNumber>
		=> new(maximum =>
		{
			return new NumberToleranceResult<TNumber, IThat<TNumber>>(
				subject.Get().ExpectationBuilder.AddConstraint(
					(Minimum: minimum, Maximum: maximum),
					static (state, it, grammars) =>
						new IsBetweenConstraint<TNumber>(it, grammars, state.Minimum, state.Maximum)
							.Invert(),
					out IsBetweenConstraint<TNumber> constraint),
				subject,
				constraint);
		});

	/// <summary>
	///     Verifies that the subject is not between the <paramref name="minimum" />…
	/// </summary>
	/// <remarks>
	///     Both bounds are inclusive. A maximum below the <paramref name="minimum" /> or a <c>NaN</c> bound throws an
	///     <see cref="ArgumentOutOfRangeException" />, while a <see langword="null" /> bound fails the expectation as
	///     well as its negation. A <c>NaN</c> subject is never between them.
	/// </remarks>
	[GuaranteesNotNull]
	public static BetweenResult<NumberToleranceResult<TNumber, IThat<TNumber?>>, TNumber?> IsNotBetween<TNumber>(
		this IThat<TNumber?> subject, TNumber? minimum)
		where TNumber : struct, INumber<TNumber>
		=> new(maximum =>
		{
			return new NumberToleranceResult<TNumber, IThat<TNumber?>>(
				subject.Get().ExpectationBuilder.AddConstraint(
					(Minimum: minimum, Maximum: maximum),
					static (state, it, grammars) =>
						new NullableIsBetweenConstraint<TNumber>(it, grammars, state.Minimum, state.Maximum).Invert(),
					out NullableIsBetweenConstraint<TNumber> constraint),
				subject,
				constraint);
		});

	private sealed class IsBetweenConstraint<TNumber> : OrderingConstraint<TNumber>,
		IValueConstraint<TNumber>,
		IOptionsHolderSource<NumberTolerance<TNumber>>
		where TNumber : struct, INumber<TNumber>
	{
		private readonly TNumber? _maximum;
		private readonly TNumber? _minimum;

		private OptionsHolder<NumberTolerance<TNumber>>? _optionsHolder;

		private NumberTolerance<TNumber> CurrentOptions
			=> _optionsHolder is null ? new NumberTolerance<TNumber>(CalculateDifference) : _optionsHolder.Value;

		/// <inheritdoc cref="IOptionsHolderSource{TOptions}.Holder" />
		OptionsHolder<NumberTolerance<TNumber>> IOptionsHolderSource<NumberTolerance<TNumber>>.Holder
			=> _optionsHolder ??= new OptionsHolder<NumberTolerance<TNumber>>(new NumberTolerance<TNumber>(CalculateDifference));

		public IsBetweenConstraint(string it,
			ExpectationGrammars grammars,
			TNumber? minimum,
			TNumber? maximum) : base(it, grammars, minimum is null || maximum is null)
		{
			minimum.ThrowIfNaN();
			maximum.ThrowIfNaN();
			if (maximum < minimum)
			{
				// ReSharper disable once LocalizableElement
				throw Tracing.WriteException(
					new ArgumentOutOfRangeException(nameof(maximum),
						"The maximum must be greater than or equal to the minimum."));
			}

			_minimum = minimum;
			_maximum = maximum;
		}

		public ConstraintResult IsMetBy(TNumber actual)
		{
			Actual = actual;
			if (IsIncomparable)
			{
				Outcome = Outcome.FailureBothWays;
				return this;
			}

			Outcome = CurrentOptions.IsInRange(actual, _minimum, _maximum) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is between ", "are between "));
			Formatter.Format(stringBuilder, _minimum);
			stringBuilder.Append(" and ");
			Formatter.Format(stringBuilder, _maximum);
			stringBuilder.Append(CurrentOptions);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
			AppendDifferenceToRange(stringBuilder, Actual, _minimum, _maximum);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not between ", "are not between "));
			Formatter.Format(stringBuilder, _minimum);
			stringBuilder.Append(" and ");
			Formatter.Format(stringBuilder, _maximum);
			stringBuilder.Append(CurrentOptions);
		}
	}

	private sealed class NullableIsBetweenConstraint<TNumber> : OrderingConstraint<TNumber?>,
		IValueConstraint<TNumber?>,
		IOptionsHolderSource<NumberTolerance<TNumber>>
		where TNumber : struct, INumber<TNumber>
	{
		private readonly TNumber? _maximum;
		private readonly TNumber? _minimum;

		private OptionsHolder<NumberTolerance<TNumber>>? _optionsHolder;

		private NumberTolerance<TNumber> CurrentOptions
			=> _optionsHolder is null ? new NumberTolerance<TNumber>(CalculateDifference) : _optionsHolder.Value;

		/// <inheritdoc cref="IOptionsHolderSource{TOptions}.Holder" />
		OptionsHolder<NumberTolerance<TNumber>> IOptionsHolderSource<NumberTolerance<TNumber>>.Holder
			=> _optionsHolder ??= new OptionsHolder<NumberTolerance<TNumber>>(new NumberTolerance<TNumber>(CalculateDifference));

		public NullableIsBetweenConstraint(string it,
			ExpectationGrammars grammars,
			TNumber? minimum,
			TNumber? maximum) : base(it, grammars, minimum is null || maximum is null)
		{
			minimum.ThrowIfNaN();
			maximum.ThrowIfNaN();
			if (maximum < minimum)
			{
				// ReSharper disable once LocalizableElement
				throw Tracing.WriteException(
					new ArgumentOutOfRangeException(nameof(maximum),
						"The maximum must be greater than or equal to the minimum."));
			}

			_minimum = minimum;
			_maximum = maximum;
		}

		public ConstraintResult IsMetBy(TNumber? actual)
		{
			Actual = actual;
			if (IsIncomparable)
			{
				Outcome = Outcome.FailureBothWays;
				return this;
			}

			Outcome = CurrentOptions.IsInRange(actual, _minimum, _maximum) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is between ", "are between "));
			Formatter.Format(stringBuilder, _minimum);
			stringBuilder.Append(" and ");
			Formatter.Format(stringBuilder, _maximum);
			stringBuilder.Append(CurrentOptions);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
			AppendDifferenceToRange(stringBuilder, Actual, _minimum, _maximum);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not between ", "are not between "));
			Formatter.Format(stringBuilder, _minimum);
			stringBuilder.Append(" and ");
			Formatter.Format(stringBuilder, _maximum);
			stringBuilder.Append(CurrentOptions);
		}
	}
#else
	private const string IsBetweenSummary =
		"Verifies that the subject is between the <paramref name=\"minimum\" />…";

	private const string IsNotBetweenSummary =
		"Verifies that the subject is not between the <paramref name=\"minimum\" />…";

	private const string IsBetweenRemarks =
		"Both bounds are inclusive. A maximum below the <paramref name=\"minimum\" /> or a <c>NaN</c> bound throws\n" +
		"an <see cref=\"System.ArgumentOutOfRangeException\" />, while a <see langword=\"null\" /> bound fails the\n" +
		"expectation as well as its negation. A <c>NaN</c> subject is never between them.";

	[CreateExpectationFamily("Is{Not}Between", Factory = typeof(NumberToleranceFactory),
		Summary = IsBetweenSummary, NegatedSummary = IsNotBetweenSummary, Remarks = IsBetweenRemarks)]
	internal static BetweenResult<NumberToleranceResult<TNumber, IThat<TNumber>>, TNumber?> IsBetweenCore<TNumber>(
		IThat<TNumber> subject,
		TNumber? minimum,
		NumberTolerance<TNumber> initialOptions,
		bool negated)
		where TNumber : struct, IComparable<TNumber>
	{
		NumberTolerance<TNumber> options = initialOptions;
		return new(maximum => new NumberToleranceResult<TNumber, IThat<TNumber>>(
			subject.Get().ExpectationBuilder.AddConstraint(
				(Minimum: minimum, Maximum: maximum, Options: options, Negated: negated),
				static (state, it, grammars) =>
					new IsBetweenConstraint<TNumber>(it, grammars, state.Minimum, state.Maximum, state.Options)
						.InvertIf(state.Negated),
				out IsBetweenConstraint<TNumber> constraint),
			subject,
			constraint));
	}

	[CreateExpectationFamily("Is{Not}Between", Factory = typeof(NumberToleranceFactory), GuaranteesNotNull = true,
		Summary = IsBetweenSummary, NegatedSummary = IsNotBetweenSummary, Remarks = IsBetweenRemarks)]
	internal static BetweenResult<NumberToleranceResult<TNumber, IThat<TNumber?>>, TNumber?>
		IsBetweenForNullableCore<TNumber>(
			IThat<TNumber?> subject,
			TNumber? minimum,
			NumberTolerance<TNumber> initialOptions,
			bool negated)
		where TNumber : struct, IComparable<TNumber>
	{
		NumberTolerance<TNumber> options = initialOptions;
		return new(maximum => new NumberToleranceResult<TNumber, IThat<TNumber?>>(
			subject.Get().ExpectationBuilder.AddConstraint(
				(Minimum: minimum, Maximum: maximum, Options: options, Negated: negated),
				static (state, it, grammars) =>
					new NullableIsBetweenConstraint<TNumber>(it, grammars, state.Minimum, state.Maximum, state.Options)
						.InvertIf(state.Negated),
				out NullableIsBetweenConstraint<TNumber> constraint),
			subject,
			constraint));
	}

	private sealed class IsBetweenConstraint<TNumber> : OrderingConstraint<TNumber>,
		IValueConstraint<TNumber>,
		IOptionsHolderSource<NumberTolerance<TNumber>>
		where TNumber : struct, IComparable<TNumber>
	{
		private readonly TNumber? _maximum;
		private readonly TNumber? _minimum;
		private readonly NumberTolerance<TNumber> _options;

		private OptionsHolder<NumberTolerance<TNumber>>? _optionsHolder;

		private NumberTolerance<TNumber> CurrentOptions => _optionsHolder is null ? _options : _optionsHolder.Value;

		/// <inheritdoc cref="IOptionsHolderSource{TOptions}.Holder" />
		OptionsHolder<NumberTolerance<TNumber>> IOptionsHolderSource<NumberTolerance<TNumber>>.Holder
			=> _optionsHolder ??= new OptionsHolder<NumberTolerance<TNumber>>(_options);

		public IsBetweenConstraint(string it,
			ExpectationGrammars grammars,
			TNumber? minimum,
			TNumber? maximum,
			NumberTolerance<TNumber> options) : base(it, grammars, minimum is null || maximum is null)
		{
			minimum.ThrowIfNaN();
			maximum.ThrowIfNaN();
			if (maximum != null && minimum != null &&
			    maximum.Value.CompareTo(minimum.Value) < 0)
			{
				// ReSharper disable once LocalizableElement
				throw Tracing.WriteException(
					new ArgumentOutOfRangeException(nameof(maximum),
						"The maximum must be greater than or equal to the minimum."));
			}

			_minimum = minimum;
			_maximum = maximum;
			_options = options;
		}

		public ConstraintResult IsMetBy(TNumber actual)
		{
			Actual = actual;
			if (IsIncomparable)
			{
				Outcome = Outcome.FailureBothWays;
				return this;
			}

			Outcome = CurrentOptions.IsInRange(actual, _minimum, _maximum) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is between ", "are between "));
			Formatter.Format(stringBuilder, _minimum);
			stringBuilder.Append(" and ");
			Formatter.Format(stringBuilder, _maximum);
			stringBuilder.Append(CurrentOptions);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
			AppendDifferenceToRange(stringBuilder, Actual, _minimum, _maximum, CurrentOptions);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not between ", "are not between "));
			Formatter.Format(stringBuilder, _minimum);
			stringBuilder.Append(" and ");
			Formatter.Format(stringBuilder, _maximum);
			stringBuilder.Append(CurrentOptions);
		}
	}

	private sealed class NullableIsBetweenConstraint<TNumber> : OrderingConstraint<TNumber?>,
		IValueConstraint<TNumber?>,
		IOptionsHolderSource<NumberTolerance<TNumber>>
		where TNumber : struct, IComparable<TNumber>
	{
		private readonly TNumber? _maximum;
		private readonly TNumber? _minimum;
		private readonly NumberTolerance<TNumber> _options;

		private OptionsHolder<NumberTolerance<TNumber>>? _optionsHolder;

		private NumberTolerance<TNumber> CurrentOptions => _optionsHolder is null ? _options : _optionsHolder.Value;

		/// <inheritdoc cref="IOptionsHolderSource{TOptions}.Holder" />
		OptionsHolder<NumberTolerance<TNumber>> IOptionsHolderSource<NumberTolerance<TNumber>>.Holder
			=> _optionsHolder ??= new OptionsHolder<NumberTolerance<TNumber>>(_options);

		public NullableIsBetweenConstraint(string it,
			ExpectationGrammars grammars,
			TNumber? minimum,
			TNumber? maximum,
			NumberTolerance<TNumber> options) : base(it, grammars, minimum is null || maximum is null)
		{
			minimum.ThrowIfNaN();
			maximum.ThrowIfNaN();
			if (maximum != null && minimum != null &&
			    maximum.Value.CompareTo(minimum.Value) < 0)
			{
				// ReSharper disable once LocalizableElement
				throw Tracing.WriteException(
					new ArgumentOutOfRangeException(nameof(maximum),
						"The maximum must be greater than or equal to the minimum."));
			}

			_minimum = minimum;
			_maximum = maximum;
			_options = options;
		}

		public ConstraintResult IsMetBy(TNumber? actual)
		{
			Actual = actual;
			if (IsIncomparable)
			{
				Outcome = Outcome.FailureBothWays;
				return this;
			}

			Outcome = CurrentOptions.IsInRange(actual, _minimum, _maximum) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is between ", "are between "));
			Formatter.Format(stringBuilder, _minimum);
			stringBuilder.Append(" and ");
			Formatter.Format(stringBuilder, _maximum);
			stringBuilder.Append(CurrentOptions);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
			AppendDifferenceToRange(stringBuilder, Actual, _minimum, _maximum, CurrentOptions);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not between ", "are not between "));
			Formatter.Format(stringBuilder, _minimum);
			stringBuilder.Append(" and ");
			Formatter.Format(stringBuilder, _maximum);
			stringBuilder.Append(CurrentOptions);
		}
	}
#endif
}
