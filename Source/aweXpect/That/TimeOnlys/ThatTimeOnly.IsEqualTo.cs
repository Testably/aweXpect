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
	///     Verifies that the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	public static TimeToleranceResult<TimeOnly, IThat<TimeOnly>> IsEqualTo(this IThat<TimeOnly> subject,
		TimeOnly? expected)
	{
		TimeTolerance tolerance = new();
		return new TimeToleranceResult<TimeOnly, IThat<TimeOnly>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToConstraint(it, grammars, expected, tolerance)),
			subject,
			tolerance);
	}

	/// <summary>
	///     Verifies that the subject is not equal to the <paramref name="unexpected" /> value.
	/// </summary>
	public static TimeToleranceResult<TimeOnly, IThat<TimeOnly>> IsNotEqualTo(
		this IThat<TimeOnly> subject,
		TimeOnly? unexpected)
	{
		TimeTolerance tolerance = new();
		return new TimeToleranceResult<TimeOnly, IThat<TimeOnly>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToConstraint(it, grammars, unexpected, tolerance).Invert()),
			subject,
			tolerance);
	}

	private sealed class IsEqualToConstraint(
		string it,
		ExpectationGrammars grammars,
		TimeOnly? expected,
		TimeTolerance tolerance)
		: ConstraintResult.WithEqualToValue<TimeOnly>(it, grammars, expected is null),
			IValueConstraint<TimeOnly>
	{
		public ConstraintResult IsMetBy(TimeOnly actual)
		{
			Actual = actual;
			if (expected is null)
			{
				Outcome = Outcome.Failure;
			}
			else
			{
				TimeSpan timeTolerance = tolerance.Tolerance ??
				                         Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Get();
				Outcome = Math.Abs(actual.Ticks - expected.Value.Ticks) <= timeTolerance.Ticks
					? Outcome.Success
					: Outcome.Failure;
			}

			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is equal to ");
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(tolerance);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is not equal to ");
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(tolerance);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
#endif
