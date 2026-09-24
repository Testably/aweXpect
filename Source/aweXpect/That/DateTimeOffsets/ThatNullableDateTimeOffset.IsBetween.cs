using System;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Customization;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatNullableDateTimeOffset
{
	/// <summary>
	///     Verifies that the subject is between the <paramref name="minimum" />…
	/// </summary>
	/// <remarks>
	///     Both bounds are inclusive. A maximum below the <paramref name="minimum" /> throws an
	///     <see cref="ArgumentOutOfRangeException" />, while a <see langword="null" /> bound fails the expectation as
	///     well as its negation.
	/// </remarks>
	[GuaranteesNotNull]
	public static BetweenResult<TimeToleranceResult<DateTimeOffset?, IThat<DateTimeOffset?>>, DateTimeOffset?>
		IsBetween(
			this IThat<DateTimeOffset?> subject,
			DateTimeOffset? minimum)
	{
		TimeTolerance tolerance = new();
		return new BetweenResult<TimeToleranceResult<DateTimeOffset?, IThat<DateTimeOffset?>>, DateTimeOffset?>(
			maximum =>
			{
				ThrowHelper.ThrowIfMaximumIsBelowMinimum(minimum, maximum);
				return new TimeToleranceResult<DateTimeOffset?, IThat<DateTimeOffset?>>(
					subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
						new IsBetweenConstraint(it, grammars, minimum, maximum, tolerance)),
					subject,
					tolerance);
			});
	}

	/// <summary>
	///     Verifies that the subject is not between the <paramref name="minimum" />…
	/// </summary>
	/// <remarks>
	///     Both bounds are inclusive. A maximum below the <paramref name="minimum" /> throws an
	///     <see cref="ArgumentOutOfRangeException" />, while a <see langword="null" /> bound fails the expectation as
	///     well as its negation.
	/// </remarks>
	[GuaranteesNotNull]
	public static BetweenResult<TimeToleranceResult<DateTimeOffset?, IThat<DateTimeOffset?>>, DateTimeOffset?>
		IsNotBetween(
			this IThat<DateTimeOffset?> subject,
			DateTimeOffset? minimum)
	{
		TimeTolerance tolerance = new();
		return new BetweenResult<TimeToleranceResult<DateTimeOffset?, IThat<DateTimeOffset?>>, DateTimeOffset?>(
			maximum =>
			{
				ThrowHelper.ThrowIfMaximumIsBelowMinimum(minimum, maximum);
				return new TimeToleranceResult<DateTimeOffset?, IThat<DateTimeOffset?>>(
					subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
						new IsBetweenConstraint(it, grammars, minimum, maximum, tolerance).Invert()),
					subject,
					tolerance);
			});
	}

	private sealed class IsBetweenConstraint(
		string it,
		ExpectationGrammars grammars,
		DateTimeOffset? minimum,
		DateTimeOffset? maximum,
		TimeTolerance tolerance)
		: OrderingConstraint<DateTimeOffset?>(it, grammars, minimum is null || maximum is null),
			IValueConstraint<DateTimeOffset?>
	{
		public ConstraintResult IsMetBy(DateTimeOffset? actual)
		{
			Actual = actual;
			if (actual is null || minimum is null || maximum is null)
			{
				Outcome = Outcome.Failure;
			}
			else
			{
				TimeSpan timeTolerance = tolerance.Tolerance
				                         ?? Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Get();
				Outcome = minimum - actual.Value <= timeTolerance &&
				          actual.Value - maximum <= timeTolerance
					? Outcome.Success
					: Outcome.Failure;
			}

			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is between ", "are between "));
			Formatter.Format(stringBuilder, minimum);
			stringBuilder.Append(" and ");
			Formatter.Format(stringBuilder, maximum);
			stringBuilder.Append(tolerance);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
			stringBuilder.AppendTimeDifferenceToRange(Actual?.UtcTicks, minimum?.UtcTicks, maximum?.UtcTicks);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not between ", "are not between "));
			Formatter.Format(stringBuilder, minimum);
			stringBuilder.Append(" and ");
			Formatter.Format(stringBuilder, maximum);
			stringBuilder.Append(tolerance);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
