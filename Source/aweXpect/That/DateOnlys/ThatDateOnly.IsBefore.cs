#if NET8_0_OR_GREATER
using System;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatDateOnly
{
	/// <summary>
	///     Verifies that the subject is before the <paramref name="expected" /> value.
	/// </summary>
	public static TimeToleranceResult<DateOnly, IThat<DateOnly>> IsBefore(
		this IThat<DateOnly> subject,
		DateOnly? expected)
	{
		TimeTolerance tolerance = new DayTolerance();
		return new TimeToleranceResult<DateOnly, IThat<DateOnly>>(
			subject.Get().ExpectationBuilder.AddConstraint((Expected: expected, Tolerance: tolerance),
				static (state, it, grammars) =>
					new IsBeforeConstraint(it, grammars, state.Expected, state.Tolerance)),
			subject,
			tolerance);
	}

	/// <summary>
	///     Verifies that the subject is not before the <paramref name="unexpected" /> value.
	/// </summary>
	public static TimeToleranceResult<DateOnly, IThat<DateOnly>> IsNotBefore(
		this IThat<DateOnly> subject,
		DateOnly? unexpected)
	{
		TimeTolerance tolerance = new DayTolerance();
		return new TimeToleranceResult<DateOnly, IThat<DateOnly>>(
			subject.Get().ExpectationBuilder.AddConstraint((Unexpected: unexpected, Tolerance: tolerance),
				static (state, it, grammars) =>
					new IsBeforeConstraint(it, grammars, state.Unexpected, state.Tolerance).Invert()),
			subject,
			tolerance);
	}

	private sealed class IsBeforeConstraint(
		string it,
		ExpectationGrammars grammars,
		DateOnly? expected,
		TimeTolerance tolerance)
		: OrderingConstraint<DateOnly>(it, grammars, expected is null),
			IValueConstraint<DateOnly>
	{
		public ConstraintResult IsMetBy(DateOnly actual)
		{
			Actual = actual;
			if (expected is null)
			{
				Outcome = Outcome.FailureBothWays;
			}
			else
			{
				TimeSpan timeTolerance = tolerance.GetToleranceOrDefault();
				Outcome = expected.Value.DayNumber - actual.DayNumber > -(int)timeTolerance.TotalDays
					? Outcome.Success
					: Outcome.Failure;
			}

			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is before ", "are before "));
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(tolerance.ToDayString());
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
			stringBuilder.AppendDayDifference(Actual.DayNumber - expected?.DayNumber);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not before ", "are not before "));
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(tolerance.ToDayString());
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
#endif
