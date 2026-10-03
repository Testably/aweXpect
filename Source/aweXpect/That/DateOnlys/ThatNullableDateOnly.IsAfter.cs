#if NET8_0_OR_GREATER
using System;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatNullableDateOnly
{
	/// <summary>
	///     Verifies that the subject is after the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static TimeToleranceResult<DateOnly, IThat<DateOnly?>> IsAfter(
		this IThat<DateOnly?> subject,
		DateOnly? expected)
	{
		TimeTolerance tolerance = new DayTolerance();
		return new TimeToleranceResult<DateOnly, IThat<DateOnly?>>(
			subject.Get().ExpectationBuilder.AddConstraint((Expected: expected, Tolerance: tolerance),
				static (state, it, grammars) =>
					new IsAfterConstraint(it, grammars, state.Expected, state.Tolerance)),
			subject,
			tolerance);
	}

	/// <summary>
	///     Verifies that the subject is not after the <paramref name="unexpected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static TimeToleranceResult<DateOnly, IThat<DateOnly?>> IsNotAfter(
		this IThat<DateOnly?> subject,
		DateOnly? unexpected)
	{
		TimeTolerance tolerance = new DayTolerance();
		return new TimeToleranceResult<DateOnly, IThat<DateOnly?>>(
			subject.Get().ExpectationBuilder.AddConstraint((Unexpected: unexpected, Tolerance: tolerance),
				static (state, it, grammars) =>
					new IsAfterConstraint(it, grammars, state.Unexpected, state.Tolerance).Invert()),
			subject,
			tolerance);
	}

	private sealed class IsAfterConstraint(
		string it,
		ExpectationGrammars grammars,
		DateOnly? expected,
		TimeTolerance tolerance)
		: OrderingConstraint<DateOnly?>(it, grammars, expected is null),
			IValueConstraint<DateOnly?>
	{
		public ConstraintResult IsMetBy(DateOnly? actual)
		{
			Actual = actual;
			if (actual is null || expected is null)
			{
				Outcome = Outcome.FailureBothWays;
			}
			else
			{
				TimeSpan timeTolerance = tolerance.GetToleranceOrDefault();
				Outcome = expected.Value.DayNumber - actual.Value.DayNumber < (int)timeTolerance.TotalDays
					? Outcome.Success
					: Outcome.Failure;
			}

			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is after ", "are after "));
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(tolerance.ToDayString());
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
			stringBuilder.AppendDayDifference(Actual?.DayNumber - expected?.DayNumber);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not after ", "are not after "));
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(tolerance.ToDayString());
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
#endif
