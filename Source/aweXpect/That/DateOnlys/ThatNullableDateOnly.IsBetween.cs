#if NET8_0_OR_GREATER
using System;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Customization;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatNullableDateOnly
{
	/// <summary>
	///     Verifies that the subject is between the <paramref name="minimum" />…
	/// </summary>
	[GuaranteesNotNull]
	public static BetweenResult<TimeToleranceResult<DateOnly?, IThat<DateOnly?>>, DateOnly?> IsBetween(
		this IThat<DateOnly?> subject,
		DateOnly? minimum)
	{
		TimeTolerance tolerance = new();
		return new BetweenResult<TimeToleranceResult<DateOnly?, IThat<DateOnly?>>, DateOnly?>(maximum =>
		{
			ThrowHelper.ThrowIfMaximumIsBelowMinimum(minimum, maximum);
			return new TimeToleranceResult<DateOnly?, IThat<DateOnly?>>(
				subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new IsBetweenConstraint(it, grammars, minimum, maximum, tolerance)),
				subject,
				tolerance);
		});
	}

	/// <summary>
	///     Verifies that the subject is not between the <paramref name="minimum" />…
	/// </summary>
	[GuaranteesNotNull]
	public static BetweenResult<TimeToleranceResult<DateOnly?, IThat<DateOnly?>>, DateOnly?> IsNotBetween(
		this IThat<DateOnly?> subject,
		DateOnly? minimum)
	{
		TimeTolerance tolerance = new();
		return new BetweenResult<TimeToleranceResult<DateOnly?, IThat<DateOnly?>>, DateOnly?>(maximum =>
		{
			ThrowHelper.ThrowIfMaximumIsBelowMinimum(minimum, maximum);
			return new TimeToleranceResult<DateOnly?, IThat<DateOnly?>>(
				subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new IsBetweenConstraint(it, grammars, minimum, maximum, tolerance).Invert()),
				subject,
				tolerance);
		});
	}

	private sealed class IsBetweenConstraint(
		string it,
		ExpectationGrammars grammars,
		DateOnly? minimum,
		DateOnly? maximum,
		TimeTolerance tolerance)
		: ConstraintResult.WithNotNullValue<DateOnly?>(it, grammars),
			IValueConstraint<DateOnly?>
	{
		public ConstraintResult IsMetBy(DateOnly? actual)
		{
			ThrowHelper.ThrowIfToleranceIsNotWholeDays(tolerance.Tolerance);
			Actual = actual;
			if (actual is null && minimum is null && maximum is null)
			{
				Outcome = Outcome.Success;
			}
			else if (actual is null || minimum is null || maximum is null)
			{
				Outcome = IsNegated ? Outcome.Success : Outcome.Failure;
			}
			else
			{
				TimeSpan timeTolerance = tolerance.Tolerance
				                         ?? Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Get();
				if (IsNegated)
				{
					timeTolerance = timeTolerance.Negate();
				}

				Outcome = minimum.Value.DayNumber - actual.Value.DayNumber <= (int)timeTolerance.TotalDays &&
				          actual.Value.DayNumber - maximum.Value.DayNumber <= (int)timeTolerance.TotalDays
					? Outcome.Success
					: Outcome.Failure;
			}

			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is between ", "are between "));
			ValueFormatters.Format(Formatter, stringBuilder, minimum);
			stringBuilder.Append(" and ");
			ValueFormatters.Format(Formatter, stringBuilder, maximum);
			stringBuilder.Append(tolerance.ToDayString());
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			ValueFormatters.Format(Formatter, stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not between ", "are not between "));
			ValueFormatters.Format(Formatter, stringBuilder, minimum);
			stringBuilder.Append(" and ");
			ValueFormatters.Format(Formatter, stringBuilder, maximum);
			stringBuilder.Append(tolerance.ToDayString());
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
#endif
