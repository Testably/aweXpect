using System;
using aweXpect.Core;
using aweXpect.Core.Constraints;
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
			NumberTolerance<TNumber> options = new(CalculateDifference);
			return new NumberToleranceResult<TNumber, IThat<TNumber>>(
				subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new IsInRangeConstraint<TNumber>(it, grammars, minimum, maximum, options)),
				subject,
				options);
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
	public static BetweenResult<NullableNumberToleranceResult<TNumber, IThat<TNumber?>>, TNumber?> IsBetween<TNumber>(
		this IThat<TNumber?> subject, TNumber? minimum)
		where TNumber : struct, INumber<TNumber>
		=> new(maximum =>
		{
			NumberTolerance<TNumber> options = new(CalculateDifference);
			return new NullableNumberToleranceResult<TNumber, IThat<TNumber?>>(
				subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new NullableIsInRangeConstraint<TNumber>(it, grammars, minimum, maximum, options)),
				subject,
				options);
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
			NumberTolerance<TNumber> options = new(CalculateDifference);
			return new NumberToleranceResult<TNumber, IThat<TNumber>>(
				subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new IsInRangeConstraint<TNumber>(it, grammars, minimum, maximum, options).Invert()),
				subject,
				options);
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
	public static BetweenResult<NullableNumberToleranceResult<TNumber, IThat<TNumber?>>, TNumber?> IsNotBetween<TNumber>(
		this IThat<TNumber?> subject, TNumber? minimum)
		where TNumber : struct, INumber<TNumber>
		=> new(maximum =>
		{
			NumberTolerance<TNumber> options = new(CalculateDifference);
			return new NullableNumberToleranceResult<TNumber, IThat<TNumber?>>(
				subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new NullableIsInRangeConstraint<TNumber>(it, grammars, minimum, maximum, options).Invert()),
				subject,
				options);
		});

	private sealed class IsInRangeConstraint<TNumber> : OrderingConstraint<TNumber>,
		IValueConstraint<TNumber>
		where TNumber : struct, INumber<TNumber>
	{
		private readonly string _it;
		private readonly TNumber? _maximum;
		private readonly TNumber? _minimum;
		private readonly NumberTolerance<TNumber> _options;

		public IsInRangeConstraint(string it,
			ExpectationGrammars grammars,
			TNumber? minimum,
			TNumber? maximum,
			NumberTolerance<TNumber> options) : base(it, grammars, minimum is null || maximum is null)
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

			_it = it;
			_minimum = minimum;
			_maximum = maximum;
			_options = options;
		}

		public ConstraintResult IsMetBy(TNumber actual)
		{
			Actual = actual;
			Outcome = _options.IsInRange(actual, _minimum, _maximum) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is between ", "are between "));
			Formatter.Format(stringBuilder, _minimum);
			stringBuilder.Append(" and ");
			Formatter.Format(stringBuilder, _maximum);
			stringBuilder.Append(_options);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(_it).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
			AppendDifferenceToRange(stringBuilder, Actual, _minimum, _maximum);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not between ", "are not between "));
			Formatter.Format(stringBuilder, _minimum);
			stringBuilder.Append(" and ");
			Formatter.Format(stringBuilder, _maximum);
			stringBuilder.Append(_options);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(_it).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
		}
	}

	private sealed class NullableIsInRangeConstraint<TNumber> : OrderingConstraint<TNumber?>,
		IValueConstraint<TNumber?>
		where TNumber : struct, INumber<TNumber>
	{
		private readonly TNumber? _maximum;
		private readonly TNumber? _minimum;
		private readonly NumberTolerance<TNumber> _options;

		public NullableIsInRangeConstraint(string it,
			ExpectationGrammars grammars,
			TNumber? minimum,
			TNumber? maximum,
			NumberTolerance<TNumber> options) : base(it, grammars, minimum is null || maximum is null)
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
			_options = options;
		}

		public ConstraintResult IsMetBy(TNumber? actual)
		{
			Actual = actual;
			Outcome = _options.IsInRange(actual, _minimum, _maximum) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is between ", "are between "));
			Formatter.Format(stringBuilder, _minimum);
			stringBuilder.Append(" and ");
			Formatter.Format(stringBuilder, _maximum);
			stringBuilder.Append(_options);
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
			stringBuilder.Append(_options);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
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

	[CreateCollectionExpectation("Is{Not}Between", Factory = typeof(NumberToleranceFactory),
		Summary = IsBetweenSummary, NegatedSummary = IsNotBetweenSummary, Remarks = IsBetweenRemarks)]
	internal static BetweenResult<NumberToleranceResult<TNumber, IThat<TNumber>>, TNumber?> IsBetweenCore<TNumber>(
		IThat<TNumber> subject,
		TNumber? minimum,
		NumberTolerance<TNumber> options,
		bool negated)
		where TNumber : struct, IComparable<TNumber>
		=> new(maximum => new NumberToleranceResult<TNumber, IThat<TNumber>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsInRangeConstraint<TNumber>(it, grammars, minimum, maximum, options).InvertIf(negated)),
			subject,
			options));

	[CreateCollectionExpectation("Is{Not}Between", Factory = typeof(NumberToleranceFactory), GuaranteesNotNull = true,
		Summary = IsBetweenSummary, NegatedSummary = IsNotBetweenSummary, Remarks = IsBetweenRemarks)]
	internal static BetweenResult<NullableNumberToleranceResult<TNumber, IThat<TNumber?>>, TNumber?>
		IsBetweenForNullableCore<TNumber>(
			IThat<TNumber?> subject,
			TNumber? minimum,
			NumberTolerance<TNumber> options,
			bool negated)
		where TNumber : struct, IComparable<TNumber>
		=> new(maximum => new NullableNumberToleranceResult<TNumber, IThat<TNumber?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsInRangeConstraint<TNumber>(it, grammars, minimum, maximum, options).InvertIf(negated)),
			subject,
			options));

	private sealed class IsInRangeConstraint<TNumber> : OrderingConstraint<TNumber>,
		IValueConstraint<TNumber>
		where TNumber : struct, IComparable<TNumber>
	{
		private readonly string _it;
		private readonly TNumber? _maximum;
		private readonly TNumber? _minimum;
		private readonly NumberTolerance<TNumber> _options;

		public IsInRangeConstraint(string it,
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

			_it = it;
			_minimum = minimum;
			_maximum = maximum;
			_options = options;
		}

		public ConstraintResult IsMetBy(TNumber actual)
		{
			Actual = actual;
			Outcome = _options.IsInRange(actual, _minimum, _maximum) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is between ", "are between "));
			Formatter.Format(stringBuilder, _minimum);
			stringBuilder.Append(" and ");
			Formatter.Format(stringBuilder, _maximum);
			stringBuilder.Append(_options);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(_it).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
			AppendDifferenceToRange(stringBuilder, Actual, _minimum, _maximum, _options);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not between ", "are not between "));
			Formatter.Format(stringBuilder, _minimum);
			stringBuilder.Append(" and ");
			Formatter.Format(stringBuilder, _maximum);
			stringBuilder.Append(_options);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(_it).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
		}
	}

	private sealed class NullableIsInRangeConstraint<TNumber> : OrderingConstraint<TNumber?>,
		IValueConstraint<TNumber?>
		where TNumber : struct, IComparable<TNumber>
	{
		private readonly TNumber? _maximum;
		private readonly TNumber? _minimum;
		private readonly NumberTolerance<TNumber> _options;

		public NullableIsInRangeConstraint(string it,
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
			Outcome = _options.IsInRange(actual, _minimum, _maximum) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is between ", "are between "));
			Formatter.Format(stringBuilder, _minimum);
			stringBuilder.Append(" and ");
			Formatter.Format(stringBuilder, _maximum);
			stringBuilder.Append(_options);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
			AppendDifferenceToRange(stringBuilder, Actual, _minimum, _maximum, _options);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not between ", "are not between "));
			Formatter.Format(stringBuilder, _minimum);
			stringBuilder.Append(" and ");
			Formatter.Format(stringBuilder, _maximum);
			stringBuilder.Append(_options);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
		}
	}
#endif
}
