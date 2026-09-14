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
	///     Verifies that the subject is between the <paramref name="minimum" />…
	/// </summary>
	public static BetweenResult<TimeToleranceResult<DateOnly, IThat<DateOnly>>, DateOnly?> IsBetween(
		this IThat<DateOnly> source,
		DateOnly? minimum)
	{
		TimeTolerance tolerance = new();
		return new BetweenResult<TimeToleranceResult<DateOnly, IThat<DateOnly>>, DateOnly?>(maximum
			=> new TimeToleranceResult<DateOnly, IThat<DateOnly>>(
				source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new IsBetweenConstraint(it, grammars, minimum, maximum, tolerance)),
				source,
				tolerance));
	}

	/// <summary>
	///     Verifies that the subject is not between the <paramref name="minimum" />…
	/// </summary>
	public static BetweenResult<TimeToleranceResult<DateOnly, IThat<DateOnly>>, DateOnly?> IsNotBetween(
		this IThat<DateOnly> source,
		DateOnly? minimum)
	{
		TimeTolerance tolerance = new();
		return new BetweenResult<TimeToleranceResult<DateOnly, IThat<DateOnly>>, DateOnly?>(maximum
			=> new TimeToleranceResult<DateOnly, IThat<DateOnly>>(
				source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new IsBetweenConstraint(it, grammars, minimum, maximum, tolerance).Invert()),
				source,
				tolerance));
	}

	private sealed class IsBetweenConstraint(
		string it,
		ExpectationGrammars grammars,
		DateOnly? minimum,
		DateOnly? maximum,
		TimeTolerance tolerance)
		: ConstraintResult.WithNotNullValue<DateOnly>(it, grammars),
			IValueConstraint<DateOnly>
	{
		public ConstraintResult IsMetBy(DateOnly actual)
		{
			Actual = actual;
			if (minimum is null || maximum is null)
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

				Outcome = actual.AddDays((int)timeTolerance.TotalDays) >= minimum &&
				          actual.AddDays((int)timeTolerance.Negate().TotalDays) <= maximum
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
			stringBuilder.Append(tolerance.ToDayString());
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
			stringBuilder.Append(tolerance.ToDayString());
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
#endif
