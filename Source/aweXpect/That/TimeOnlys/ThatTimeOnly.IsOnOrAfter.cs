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
	///     Verifies that the subject is on or after the <paramref name="expected" /> value.
	/// </summary>
	/// <remarks>
	///     The times are compared linearly from midnight, so 00:01 is before 23:59, while
	///     <see cref="IsEqualTo(IThat{TimeOnly}, TimeOnly?)" /> and <see cref="IsBetween(IThat{TimeOnly}, TimeOnly?)" />
	///     treat the times as a clock face that wraps around midnight.
	/// </remarks>
	public static TimeToleranceResult<TimeOnly, IThat<TimeOnly>> IsOnOrAfter(
		this IThat<TimeOnly> subject,
		TimeOnly? expected)
	{
		TimeTolerance tolerance = new();
		return new TimeToleranceResult<TimeOnly, IThat<TimeOnly>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOnOrAfterConstraint(it, grammars, expected, tolerance)),
			subject,
			tolerance);
	}

	/// <summary>
	///     Verifies that the subject is not on or after the <paramref name="unexpected" /> value.
	/// </summary>
	/// <remarks>
	///     The times are compared linearly from midnight, so 00:01 is before 23:59, while
	///     <see cref="IsEqualTo(IThat{TimeOnly}, TimeOnly?)" /> and <see cref="IsBetween(IThat{TimeOnly}, TimeOnly?)" />
	///     treat the times as a clock face that wraps around midnight.
	/// </remarks>
	public static TimeToleranceResult<TimeOnly, IThat<TimeOnly>> IsNotOnOrAfter(
		this IThat<TimeOnly> subject,
		TimeOnly? unexpected)
	{
		TimeTolerance tolerance = new();
		return new TimeToleranceResult<TimeOnly, IThat<TimeOnly>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOnOrAfterConstraint(it, grammars, unexpected, tolerance).Invert()),
			subject,
			tolerance);
	}

	private sealed class IsOnOrAfterConstraint(
		string it,
		ExpectationGrammars grammars,
		TimeOnly? expected,
		TimeTolerance tolerance)
		: ConstraintResult.WithNotNullValue<TimeOnly>(it, grammars),
			IValueConstraint<TimeOnly>
	{
		public ConstraintResult IsMetBy(TimeOnly actual)
		{
			Actual = actual;
			if (expected is null)
			{
				Outcome = IsNegated ? Outcome.Success : Outcome.Failure;
				return this;
			}

			TimeSpan timeTolerance = tolerance.Tolerance
			                         ?? Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Get();
			if (IsNegated)
			{
				timeTolerance = timeTolerance.Negate();
			}

			Outcome = expected.Value.Ticks - actual.Ticks <= timeTolerance.Ticks
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is on or after ", "are on or after "));
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
			stringBuilder.Append(Grammars.Verb("is not on or after ", "are not on or after "));
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(tolerance);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
#endif
