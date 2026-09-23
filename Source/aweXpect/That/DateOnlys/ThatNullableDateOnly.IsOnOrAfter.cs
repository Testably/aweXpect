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
	///     Verifies that the subject is on or after the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static TimeToleranceResult<DateOnly?, IThat<DateOnly?>> IsOnOrAfter(
		this IThat<DateOnly?> subject,
		DateOnly? expected)
	{
		TimeTolerance tolerance = new();
		return new TimeToleranceResult<DateOnly?, IThat<DateOnly?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOnOrAfterConstraint(it, grammars, expected, tolerance)),
			subject,
			tolerance);
	}

	/// <summary>
	///     Verifies that the subject is not on or after the <paramref name="unexpected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static TimeToleranceResult<DateOnly?, IThat<DateOnly?>> IsNotOnOrAfter(
		this IThat<DateOnly?> subject,
		DateOnly? unexpected)
	{
		TimeTolerance tolerance = new();
		return new TimeToleranceResult<DateOnly?, IThat<DateOnly?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOnOrAfterConstraint(it, grammars, unexpected, tolerance).Invert()),
			subject,
			tolerance);
	}

	private sealed class IsOnOrAfterConstraint(
		string it,
		ExpectationGrammars grammars,
		DateOnly? expected,
		TimeTolerance tolerance)
		: ConstraintResult.WithNotNullValue<DateOnly?>(it, grammars),
			IValueConstraint<DateOnly?>
	{
		public ConstraintResult IsMetBy(DateOnly? actual)
		{
			ThrowHelper.ThrowIfToleranceIsNotWholeDays(tolerance.Tolerance);
			Actual = actual;
			if (actual is null && expected is null)
			{
				Outcome = Outcome.Success;
			}
			else if (actual is null || expected is null)
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

				Outcome = expected.Value.DayNumber - actual.Value.DayNumber <= (int)timeTolerance.TotalDays
					? Outcome.Success
					: Outcome.Failure;
			}

			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is on or after ", "are on or after "));
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(tolerance.ToDayString());
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not on or after ", "are not on or after "));
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(tolerance.ToDayString());
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
#endif
