using System;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatDateTimeOffset
{
	/// <summary>
	///     Verifies that the subject is after the <paramref name="expected" /> value.
	/// </summary>
	public static TimeToleranceResult<DateTimeOffset, IThat<DateTimeOffset>> IsAfter(
		this IThat<DateTimeOffset> subject,
		DateTimeOffset? expected)
	{
		TimeTolerance tolerance = new();
		return new TimeToleranceResult<DateTimeOffset, IThat<DateTimeOffset>>(
			subject.Get().ExpectationBuilder.AddConstraint((Expected: expected, Tolerance: tolerance),
				static (state, it, grammars) =>
					new IsAfterConstraint(it, grammars, state.Expected, state.Tolerance)),
			subject,
			tolerance);
	}

	/// <summary>
	///     Verifies that the subject is not after the <paramref name="unexpected" /> value.
	/// </summary>
	public static TimeToleranceResult<DateTimeOffset, IThat<DateTimeOffset>> IsNotAfter(
		this IThat<DateTimeOffset> subject,
		DateTimeOffset? unexpected)
	{
		TimeTolerance tolerance = new();
		return new TimeToleranceResult<DateTimeOffset, IThat<DateTimeOffset>>(
			subject.Get().ExpectationBuilder.AddConstraint((Unexpected: unexpected, Tolerance: tolerance),
				static (state, it, grammars) =>
					new IsAfterConstraint(it, grammars, state.Unexpected, state.Tolerance).Invert()),
			subject,
			tolerance);
	}

	private sealed class IsAfterConstraint(
		string it,
		ExpectationGrammars grammars,
		DateTimeOffset? expected,
		TimeTolerance tolerance)
		: OrderingConstraint<DateTimeOffset>(it, grammars, expected is null),
			IValueConstraint<DateTimeOffset>
	{
		public ConstraintResult IsMetBy(DateTimeOffset actual)
		{
			Actual = actual;
			if (expected is null)
			{
				Outcome = Outcome.FailureBothWays;
			}
			else
			{
				TimeSpan timeTolerance = tolerance.GetToleranceOrDefault();
				Outcome = expected - actual < timeTolerance ? Outcome.Success : Outcome.Failure;
			}

			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is after ", "are after "));
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(tolerance);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
			stringBuilder.AppendTimeDifference((Actual - expected)?.Ticks);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not after ", "are not after "));
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(tolerance);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
