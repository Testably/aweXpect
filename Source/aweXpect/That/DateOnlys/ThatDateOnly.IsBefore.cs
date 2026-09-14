#if NET8_0_OR_GREATER
using System;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Customization;
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
		this IThat<DateOnly> source,
		DateOnly? expected)
	{
		TimeTolerance tolerance = new();
		return new TimeToleranceResult<DateOnly, IThat<DateOnly>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsBeforeConstraint(it, grammars, expected, tolerance)),
			source,
			tolerance);
	}

	/// <summary>
	///     Verifies that the subject is not before the <paramref name="unexpected" /> value.
	/// </summary>
	public static TimeToleranceResult<DateOnly, IThat<DateOnly>> IsNotBefore(
		this IThat<DateOnly> source,
		DateOnly? unexpected)
	{
		TimeTolerance tolerance = new();
		return new TimeToleranceResult<DateOnly, IThat<DateOnly>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsBeforeConstraint(it, grammars, unexpected, tolerance).Invert()),
			source,
			tolerance);
	}

	private sealed class IsBeforeConstraint(
		string it,
		ExpectationGrammars grammars,
		DateOnly? expected,
		TimeTolerance tolerance)
		: ConstraintResult.WithNotNullValue<DateOnly>(it, grammars),
			IValueConstraint<DateOnly>
	{
		public ConstraintResult IsMetBy(DateOnly actual)
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

				Outcome = actual.AddDays((int)timeTolerance.TotalDays) < expected ? Outcome.Success : Outcome.Failure;
			}

			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is before ");
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(tolerance.ToDayString());
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
			stringBuilder.Append(tolerance.ToDayString());
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
#endif
