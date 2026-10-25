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
	///     Verifies that the subject is between the <paramref name="minimum" />…
	/// </summary>
	/// <remarks>
	///     Both bounds are inclusive. A maximum below the <paramref name="minimum" /> throws an
	///     <see cref="ArgumentOutOfRangeException" />, while a <see langword="null" /> bound fails the expectation as
	///     well as its negation.
	/// </remarks>
	public static BetweenResult<TimeToleranceResult<DateOnly, IThat<DateOnly>>, DateOnly?> IsBetween(
		this IThat<DateOnly> subject,
		DateOnly? minimum)
	{
		return new BetweenResult<TimeToleranceResult<DateOnly, IThat<DateOnly>>, DateOnly?>(maximum =>
		{
			ThrowHelper.ThrowIfMaximumIsBelowMinimum(minimum, maximum);
			return new TimeToleranceResult<DateOnly, IThat<DateOnly>>(
				subject.Get().ExpectationBuilder.AddConstraint(
					(Minimum: minimum, Maximum: maximum),
					static (state, it, grammars) =>
						new IsBetweenConstraint(it, grammars, state.Minimum, state.Maximum),
					out IsBetweenConstraint constraint),
				subject,
				constraint);
		});
	}

	/// <summary>
	///     Verifies that the subject is not between the <paramref name="minimum" />…
	/// </summary>
	/// <remarks>
	///     Both bounds are inclusive. A maximum below the <paramref name="minimum" /> throws an
	///     <see cref="ArgumentOutOfRangeException" />, while a <see langword="null" /> bound fails the expectation as
	///     well as its negation.
	/// </remarks>
	public static BetweenResult<TimeToleranceResult<DateOnly, IThat<DateOnly>>, DateOnly?> IsNotBetween(
		this IThat<DateOnly> subject,
		DateOnly? minimum)
	{
		return new BetweenResult<TimeToleranceResult<DateOnly, IThat<DateOnly>>, DateOnly?>(maximum =>
		{
			ThrowHelper.ThrowIfMaximumIsBelowMinimum(minimum, maximum);
			return new TimeToleranceResult<DateOnly, IThat<DateOnly>>(
				subject.Get().ExpectationBuilder.AddConstraint(
					(Minimum: minimum, Maximum: maximum),
					static (state, it, grammars) =>
						new IsBetweenConstraint(it, grammars, state.Minimum, state.Maximum).Invert(),
					out IsBetweenConstraint constraint),
				subject,
				constraint);
		});
	}

	private sealed class IsBetweenConstraint(
		string it,
		ExpectationGrammars grammars,
		DateOnly? minimum,
		DateOnly? maximum)
		: OrderingConstraint<DateOnly>(it, grammars, minimum is null || maximum is null),
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
			if (minimum is null || maximum is null)
			{
				Outcome = Outcome.FailureBothWays;
			}
			else
			{
				TimeSpan timeTolerance = CurrentTolerance.GetToleranceOrDefault();
				Outcome = minimum.Value.DayNumber - actual.DayNumber <= (int)timeTolerance.TotalDays &&
				          actual.DayNumber - maximum.Value.DayNumber <= (int)timeTolerance.TotalDays
					? Outcome.Success
					: Outcome.Failure;
			}

			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is between ", "are between "));
			Formatter.Format(stringBuilder, minimum);
			stringBuilder.Append(" and ");
			Formatter.Format(stringBuilder, maximum);
			stringBuilder.Append(CurrentTolerance.ToDayString());
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
			stringBuilder.AppendDayDifferenceToRange(Actual.DayNumber, minimum?.DayNumber, maximum?.DayNumber);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not between ", "are not between "));
			Formatter.Format(stringBuilder, minimum);
			stringBuilder.Append(" and ");
			Formatter.Format(stringBuilder, maximum);
			stringBuilder.Append(CurrentTolerance.ToDayString());
		}
	}
}
#endif
