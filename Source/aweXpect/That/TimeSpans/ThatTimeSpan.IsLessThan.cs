using System;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatTimeSpan
{
	/// <summary>
	///     Verifies that the subject is less than the <paramref name="expected" /> value.
	/// </summary>
	public static TimeToleranceResult<TimeSpan, IThat<TimeSpan>> IsLessThan(
		this IThat<TimeSpan> subject,
		TimeSpan? expected)
	{
		OptionsHolder<TimeTolerance> tolerance = new(TimeTolerance.Default);
		return new TimeToleranceResult<TimeSpan, IThat<TimeSpan>>(
			subject.Get().ExpectationBuilder.AddConstraint((Expected: expected, Tolerance: tolerance),
				static (state, it, grammars) =>
					new IsLessThanConstraint(it, grammars, state.Expected, state.Tolerance)),
			subject,
			tolerance);
	}

	/// <summary>
	///     Verifies that the subject is not less than the <paramref name="unexpected" /> value.
	/// </summary>
	public static TimeToleranceResult<TimeSpan, IThat<TimeSpan>> IsNotLessThan(
		this IThat<TimeSpan> subject,
		TimeSpan? unexpected)
	{
		OptionsHolder<TimeTolerance> tolerance = new(TimeTolerance.Default);
		return new TimeToleranceResult<TimeSpan, IThat<TimeSpan>>(
			subject.Get().ExpectationBuilder.AddConstraint((Unexpected: unexpected, Tolerance: tolerance),
				static (state, it, grammars) =>
					new IsLessThanConstraint(it, grammars, state.Unexpected, state.Tolerance).Invert()),
			subject,
			tolerance);
	}

	private sealed class IsLessThanConstraint(
		string it,
		ExpectationGrammars grammars,
		TimeSpan? expected,
		OptionsHolder<TimeTolerance> tolerance)
		: OrderingConstraint<TimeSpan>(it, grammars, expected is null),
			IValueConstraint<TimeSpan>
	{
		public ConstraintResult IsMetBy(TimeSpan actual)
		{
			Actual = actual;
			if (expected is null)
			{
				Outcome = Outcome.FailureBothWays;
				return this;
			}

			TimeSpan timeTolerance = tolerance.Value.GetToleranceOrDefault();
			Outcome = actual.ShiftedTicks(timeTolerance.Negate()) < expected.Value.Ticks
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is less than ", "are less than "));
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(tolerance);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
			stringBuilder.AppendTimeDifference((decimal)Actual.Ticks - expected?.Ticks);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not less than ", "are not less than "));
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(tolerance);
		}
	}
}
