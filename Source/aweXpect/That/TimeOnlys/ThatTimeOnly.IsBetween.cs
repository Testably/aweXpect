#if NET8_0_OR_GREATER
using System;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Customization;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatTimeOnly
{
	/// <summary>
	///     Verifies that the subject is between the <paramref name="minimum" />…
	/// </summary>
	/// <remarks>
	///     Both bounds are inclusive. A maximum before the <paramref name="minimum" /> describes a range across
	///     midnight, so the range from 22:00 to 02:00 includes 23:00 and 01:00. A <see langword="null" /> bound fails
	///     the expectation as well as its negation.
	/// </remarks>
	public static BetweenResult<TimeToleranceResult<TimeOnly, IThat<TimeOnly>>, TimeOnly?> IsBetween(
		this IThat<TimeOnly> subject,
		TimeOnly? minimum)
	{
		TimeTolerance tolerance = new();
		return new BetweenResult<TimeToleranceResult<TimeOnly, IThat<TimeOnly>>, TimeOnly?>(maximum
			=> new TimeToleranceResult<TimeOnly, IThat<TimeOnly>>(
				subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new IsBetweenConstraint(it, grammars, minimum, maximum, tolerance)),
				subject,
				tolerance));
	}

	/// <summary>
	///     Verifies that the subject is not between the <paramref name="minimum" />…
	/// </summary>
	/// <remarks>
	///     Both bounds are inclusive. A maximum before the <paramref name="minimum" /> describes a range across
	///     midnight, so the range from 22:00 to 02:00 includes 23:00 and 01:00. A <see langword="null" /> bound fails
	///     the expectation as well as its negation.
	/// </remarks>
	public static BetweenResult<TimeToleranceResult<TimeOnly, IThat<TimeOnly>>, TimeOnly?> IsNotBetween(
		this IThat<TimeOnly> subject,
		TimeOnly? minimum)
	{
		TimeTolerance tolerance = new();
		return new BetweenResult<TimeToleranceResult<TimeOnly, IThat<TimeOnly>>, TimeOnly?>(maximum
			=> new TimeToleranceResult<TimeOnly, IThat<TimeOnly>>(
				subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new IsBetweenConstraint(it, grammars, minimum, maximum, tolerance).Invert()),
				subject,
				tolerance));
	}

	private sealed class IsBetweenConstraint(
		string it,
		ExpectationGrammars grammars,
		TimeOnly? minimum,
		TimeOnly? maximum,
		TimeTolerance tolerance)
		: OrderingConstraint<TimeOnly>(it, grammars, minimum is null || maximum is null),
			IValueConstraint<TimeOnly>
	{
		public ConstraintResult IsMetBy(TimeOnly actual)
		{
			Actual = actual;
			if (minimum is null || maximum is null)
			{
				Outcome = Outcome.Failure;
			}
			else
			{
				TimeSpan timeTolerance = tolerance.Tolerance
				                         ?? Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Get();
				Outcome = actual.IsOnArc(minimum.Value, maximum.Value, timeTolerance)
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
			stringBuilder.AppendTimeDifferenceToArc(Actual, minimum, maximum);
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
#endif
