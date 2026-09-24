using System;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Customization;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatTimeSpan
{
	/// <summary>
	///     Verifies that the subject is between the <paramref name="minimum" />…
	/// </summary>
	/// <remarks>
	///     Both bounds are inclusive. A maximum below the <paramref name="minimum" /> throws an
	///     <see cref="ArgumentOutOfRangeException" />, while a <see langword="null" /> bound fails the expectation as
	///     well as its negation.
	/// </remarks>
	public static BetweenResult<TimeToleranceResult<TimeSpan, IThat<TimeSpan>>, TimeSpan?> IsBetween(
		this IThat<TimeSpan> subject,
		TimeSpan? minimum)
	{
		TimeTolerance tolerance = new();
		return new BetweenResult<TimeToleranceResult<TimeSpan, IThat<TimeSpan>>, TimeSpan?>(maximum =>
		{
			ThrowHelper.ThrowIfMaximumIsBelowMinimum(minimum, maximum);
			return new TimeToleranceResult<TimeSpan, IThat<TimeSpan>>(
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
	public static BetweenResult<TimeToleranceResult<TimeSpan, IThat<TimeSpan>>, TimeSpan?> IsNotBetween(
		this IThat<TimeSpan> subject,
		TimeSpan? minimum)
	{
		TimeTolerance tolerance = new();
		return new BetweenResult<TimeToleranceResult<TimeSpan, IThat<TimeSpan>>, TimeSpan?>(maximum =>
		{
			ThrowHelper.ThrowIfMaximumIsBelowMinimum(minimum, maximum);
			return new TimeToleranceResult<TimeSpan, IThat<TimeSpan>>(
				subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new IsBetweenConstraint(it, grammars, minimum, maximum, tolerance).Invert()),
				subject,
				tolerance);
		});
	}

	private sealed class IsBetweenConstraint(
		string it,
		ExpectationGrammars grammars,
		TimeSpan? minimum,
		TimeSpan? maximum,
		TimeTolerance tolerance)
		: ConstraintResult.WithNotNullValue<TimeSpan>(it, grammars),
			IValueConstraint<TimeSpan>
	{
		public ConstraintResult IsMetBy(TimeSpan actual)
		{
			Actual = actual;
			if (minimum is null || maximum is null)
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

				Outcome = actual.ShiftedTicks(timeTolerance) >= minimum.Value.Ticks &&
				          actual.ShiftedTicks(timeTolerance.Negate()) <= maximum.Value.Ticks
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
			stringBuilder.AppendTimeDifferenceToRange(Actual.Ticks, minimum?.Ticks, maximum?.Ticks);
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
