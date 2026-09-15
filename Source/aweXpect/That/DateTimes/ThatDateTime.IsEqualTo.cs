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
	///     Verifies that the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	public static TimeToleranceResult<DateTime, IThat<DateTime>> IsEqualTo(
		this IThat<DateTime> subject,
		DateTime? expected)
	{
		TimeTolerance tolerance = new();
		return new TimeToleranceResult<DateTime, IThat<DateTime>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToConstraint(it, grammars, expected, tolerance)),
			subject,
			tolerance);
	}

	/// <summary>
	///     Verifies that the subject is not equal to the <paramref name="unexpected" /> value.
	/// </summary>
	public static TimeToleranceResult<DateTime, IThat<DateTime>> IsNotEqualTo(
		this IThat<DateTime> subject,
		DateTime? unexpected)
	{
		TimeTolerance tolerance = new();
		return new TimeToleranceResult<DateTime, IThat<DateTime>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToConstraint(it, grammars, unexpected, tolerance).Invert()),
			subject,
			tolerance);
	}

	private sealed class IsEqualToConstraint(
		string it,
		ExpectationGrammars grammars,
		DateTime? expected,
		TimeTolerance tolerance)
		: ConstraintResult.WithEqualToValue<DateTime>(it, grammars, expected is null),
			IValueConstraint<DateTime>
	{
		private bool _hasKindDifference;

		public ConstraintResult IsMetBy(DateTime actual)
		{
			Actual = actual;

			TimeSpan timeTolerance =
				tolerance.Tolerance ?? Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Get();
			Outcome = actual.IsConsideredEqualTo(expected, timeTolerance, out _hasKindDifference)
				? Outcome.Success
				: Outcome.Failure;

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
			if (_hasKindDifference)
			{
				stringBuilder.Append(It).Append(" differed in the Kind property");
			}
			else
			{
				stringBuilder.Append(It).Append(" was ");
				Formatter.Format(stringBuilder, Actual);
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is not equal to ");
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(tolerance);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
		}
	}
}
