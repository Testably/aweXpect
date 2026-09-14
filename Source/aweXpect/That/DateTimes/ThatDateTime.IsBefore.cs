using System;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Customization;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatDateTime
{
	/// <summary>
	///     Verifies that the subject is before the <paramref name="expected" /> value.
	/// </summary>
	public static TimeToleranceResult<DateTime, IThat<DateTime>> IsBefore(
		this IThat<DateTime> source,
		DateTime? expected)
	{
		TimeTolerance tolerance = new();
		return new TimeToleranceResult<DateTime, IThat<DateTime>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsBeforeConstraint(it, grammars, expected, tolerance)),
			source,
			tolerance);
	}

	/// <summary>
	///     Verifies that the subject is not before the <paramref name="unexpected" /> value.
	/// </summary>
	public static TimeToleranceResult<DateTime, IThat<DateTime>> IsNotBefore(
		this IThat<DateTime> source,
		DateTime? unexpected)
	{
		TimeTolerance tolerance = new();
		return new TimeToleranceResult<DateTime, IThat<DateTime>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsBeforeConstraint(it, grammars, unexpected, tolerance).Invert()),
			source,
			tolerance);
	}

	private sealed class IsBeforeConstraint(
		string it,
		ExpectationGrammars grammars,
		DateTime? expected,
		TimeTolerance tolerance)
		: ConstraintResult.WithNotNullValue<DateTime>(it, grammars),
			IValueConstraint<DateTime>
	{
		public ConstraintResult IsMetBy(DateTime actual)
		{
			Actual = actual;
			if (expected is null)
			{
				Outcome = Outcome.Failure;
			}
			else
			{
				TimeSpan timeTolerance = tolerance.Tolerance
				                         ?? Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Get();
				if (!IsNegated)
				{
					timeTolerance = timeTolerance.Negate();
				}

				Outcome = actual.Add(timeTolerance) < expected ? Outcome.Success : Outcome.Failure;
			}

			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is before ");
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
			stringBuilder.Append("is not before ");
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(tolerance);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
