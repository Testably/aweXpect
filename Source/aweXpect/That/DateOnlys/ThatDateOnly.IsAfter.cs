#if NET8_0_OR_GREATER
using System;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatDateOnly
{
	/// <summary>
	///     Verifies that the subject is after the <paramref name="expected" /> value.
	/// </summary>
	public static TimeToleranceResult<DateOnly, IThat<DateOnly>> IsAfter(
		this IThat<DateOnly> subject,
		DateOnly? expected)
	{
		return new TimeToleranceResult<DateOnly, IThat<DateOnly>>(
			subject.Get().ExpectationBuilder.AddConstraint(expected,
				static (expected, it, grammars) =>
					new IsAfterConstraint(it, grammars, expected),
				out IsAfterConstraint constraint),
			subject,
			constraint);
	}

	/// <summary>
	///     Verifies that the subject is not after the <paramref name="unexpected" /> value.
	/// </summary>
	public static TimeToleranceResult<DateOnly, IThat<DateOnly>> IsNotAfter(
		this IThat<DateOnly> subject,
		DateOnly? unexpected)
	{
		return new TimeToleranceResult<DateOnly, IThat<DateOnly>>(
			subject.Get().ExpectationBuilder.AddConstraint(unexpected,
				static (unexpected, it, grammars) =>
					new IsAfterConstraint(it, grammars, unexpected).Invert(),
				out IsAfterConstraint constraint),
			subject,
			constraint);
	}

	private sealed class IsAfterConstraint(
		string it,
		ExpectationGrammars grammars,
		DateOnly? expected)
		: OrderingConstraint<DateOnly>(it, grammars, expected is null),
			IValueConstraint<DateOnly>,
			IOptionsHolderSource<TimeTolerance>
	{
		private OptionsHolder<TimeTolerance>? _toleranceHolder;

		private TimeTolerance CurrentTolerance
			=> _toleranceHolder is null ? TimeTolerance.Days : _toleranceHolder.Value;

		/// <inheritdoc cref="IOptionsHolderSource{TOptions}.Holder" />
		OptionsHolder<TimeTolerance> IOptionsHolderSource<TimeTolerance>.Holder
			=> _toleranceHolder ??= new OptionsHolder<TimeTolerance>(TimeTolerance.Days);

		public ConstraintResult IsMetBy(DateOnly actual)
		{
			Actual = actual;
			if (expected is null)
			{
				Outcome = Outcome.FailureBothWays;
			}
			else
			{
				TimeSpan timeTolerance = CurrentTolerance.GetToleranceOrDefault();
				Outcome = expected.Value.DayNumber - actual.DayNumber < (int)timeTolerance.TotalDays
					? Outcome.Success
					: Outcome.Failure;
			}

			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is after ", "are after "));
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(CurrentTolerance.ToDayString());
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
			stringBuilder.AppendDayDifference(Actual.DayNumber - expected?.DayNumber);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not after ", "are not after "));
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(CurrentTolerance.ToDayString());
		}
	}
}
#endif
