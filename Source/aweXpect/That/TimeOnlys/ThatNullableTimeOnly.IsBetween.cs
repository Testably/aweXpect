#if NET8_0_OR_GREATER
using System;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Customization;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatNullableTimeOnly
{
	/// <summary>
	///     Verifies that the subject is between the <paramref name="minimum" />…
	/// </summary>
	[GuaranteesNotNull]
	public static BetweenResult<TimeToleranceResult<TimeOnly?, IThat<TimeOnly?>>, TimeOnly?> IsBetween(
		this IThat<TimeOnly?> source,
		TimeOnly? minimum)
	{
		TimeTolerance tolerance = new();
		return new BetweenResult<TimeToleranceResult<TimeOnly?, IThat<TimeOnly?>>, TimeOnly?>(maximum
			=> new TimeToleranceResult<TimeOnly?, IThat<TimeOnly?>>(
				source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new IsBetweenConstraint(it, grammars, minimum, maximum, tolerance)),
				source,
				tolerance));
	}

	/// <summary>
	///     Verifies that the subject is not between the <paramref name="minimum" />…
	/// </summary>
	public static BetweenResult<TimeToleranceResult<TimeOnly?, IThat<TimeOnly?>>, TimeOnly?> IsNotBetween(
		this IThat<TimeOnly?> source,
		TimeOnly? minimum)
	{
		TimeTolerance tolerance = new();
		return new BetweenResult<TimeToleranceResult<TimeOnly?, IThat<TimeOnly?>>, TimeOnly?>(maximum
			=> new TimeToleranceResult<TimeOnly?, IThat<TimeOnly?>>(
				source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new IsBetweenConstraint(it, grammars, minimum, maximum, tolerance).Invert()),
				source,
				tolerance));
	}

	private sealed class IsBetweenConstraint(
		string it,
		ExpectationGrammars grammars,
		TimeOnly? minimum,
		TimeOnly? maximum,
		TimeTolerance tolerance)
		: ConstraintResult.WithNotNullValue<TimeOnly?>(it, grammars),
			IValueConstraint<TimeOnly?>
	{
		public ConstraintResult IsMetBy(TimeOnly? actual)
		{
			Actual = actual;
			if (actual is null && minimum is null && maximum is null)
			{
				Outcome = Outcome.Success;
			}
			else if (actual is null || minimum is null || maximum is null)
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

				Outcome = actual.Value.Add(timeTolerance) >= minimum &&
				          actual.Value.Add(timeTolerance.Negate()) <= maximum
					? Outcome.Success
					: Outcome.Failure;
			}

			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is between ");
			ValueFormatters.Format(Formatter, stringBuilder, minimum);
			stringBuilder.Append(" and ");
			ValueFormatters.Format(Formatter, stringBuilder, maximum);
			stringBuilder.Append(tolerance);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was ");
			ValueFormatters.Format(Formatter, stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is not between ");
			ValueFormatters.Format(Formatter, stringBuilder, minimum);
			stringBuilder.Append(" and ");
			ValueFormatters.Format(Formatter, stringBuilder, maximum);
			stringBuilder.Append(tolerance);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
#endif
