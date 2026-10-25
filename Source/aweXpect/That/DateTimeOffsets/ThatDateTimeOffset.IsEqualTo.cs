using System;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatDateTimeOffset
{
	/// <summary>
	///     Verifies that the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	public static TimeToleranceResult<DateTimeOffset, IThat<DateTimeOffset>> IsEqualTo(
		this IThat<DateTimeOffset> subject,
		DateTimeOffset? expected)
	{
		OptionsSlot<TimeTolerance> tolerance = new(TimeTolerance.Default);
		return new TimeToleranceResult<DateTimeOffset, IThat<DateTimeOffset>>(
			subject.Get().ExpectationBuilder.AddConstraint((Expected: expected, Tolerance: tolerance),
				static (state, it, grammars) =>
					new IsEqualToConstraint(it, grammars, state.Expected, state.Tolerance)),
			subject,
			tolerance);
	}

	/// <summary>
	///     Verifies that the subject is not equal to the <paramref name="unexpected" /> value.
	/// </summary>
	public static TimeToleranceResult<DateTimeOffset, IThat<DateTimeOffset>> IsNotEqualTo(
		this IThat<DateTimeOffset> subject,
		DateTimeOffset? unexpected)
	{
		OptionsSlot<TimeTolerance> tolerance = new(TimeTolerance.Default);
		return new TimeToleranceResult<DateTimeOffset, IThat<DateTimeOffset>>(
			subject.Get().ExpectationBuilder.AddConstraint((Unexpected: unexpected, Tolerance: tolerance),
				static (state, it, grammars) =>
					new IsEqualToConstraint(it, grammars, state.Unexpected, state.Tolerance).Invert()),
			subject,
			tolerance);
	}

	private sealed class IsEqualToConstraint(
		string it,
		ExpectationGrammars grammars,
		DateTimeOffset? expected,
		OptionsSlot<TimeTolerance> tolerance)
		: ConstraintResult.WithEqualToValue<DateTimeOffset>(it, grammars, expected is null),
			IValueConstraint<DateTimeOffset>
	{
		public ConstraintResult IsMetBy(DateTimeOffset actual)
		{
			Actual = actual;
			if (expected is null)
			{
				Outcome = Outcome.Failure;
			}
			else
			{
				TimeSpan timeTolerance = tolerance.Value.GetToleranceOrDefault();
				Outcome = actual.IsConsideredEqualTo(expected, timeTolerance)
					? Outcome.Success
					: Outcome.Failure;
			}

			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is equal to ", "are equal to "));
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
			stringBuilder.Append(Grammars.Verb("is not equal to ", "are not equal to "));
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(tolerance);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
