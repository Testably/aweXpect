using System;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatNullableDateTimeOffset
{
	/// <summary>
	///     Verifies that the subject is between the <paramref name="minimum" />…
	/// </summary>
	/// <remarks>
	///     Both bounds are inclusive. A maximum below the <paramref name="minimum" /> throws an
	///     <see cref="ArgumentOutOfRangeException" />, while a <see langword="null" /> bound fails the expectation as
	///     well as its negation.
	/// </remarks>
	[GuaranteesNotNull]
	public static BetweenResult<TimeToleranceResult<DateTimeOffset, IThat<DateTimeOffset?>>, DateTimeOffset?>
		IsBetween(
			this IThat<DateTimeOffset?> subject,
			DateTimeOffset? minimum)
	{
		return new BetweenResult<TimeToleranceResult<DateTimeOffset, IThat<DateTimeOffset?>>, DateTimeOffset?>(maximum =>
		{
			ThrowHelper.ThrowIfMaximumIsBelowMinimum(minimum, maximum);
			return new TimeToleranceResult<DateTimeOffset, IThat<DateTimeOffset?>>(
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
	[GuaranteesNotNull]
	public static BetweenResult<TimeToleranceResult<DateTimeOffset, IThat<DateTimeOffset?>>, DateTimeOffset?>
		IsNotBetween(
			this IThat<DateTimeOffset?> subject,
			DateTimeOffset? minimum)
	{
		return new BetweenResult<TimeToleranceResult<DateTimeOffset, IThat<DateTimeOffset?>>, DateTimeOffset?>(maximum =>
		{
			ThrowHelper.ThrowIfMaximumIsBelowMinimum(minimum, maximum);
			return new TimeToleranceResult<DateTimeOffset, IThat<DateTimeOffset?>>(
				subject.Get().ExpectationBuilder.AddConstraint(
					(Minimum: minimum, Maximum: maximum),
					static (state, it, grammars) =>
						new IsBetweenConstraint(it, grammars, state.Minimum, state.Maximum)
							.Invert(),
					out IsBetweenConstraint constraint),
				subject,
				constraint);
		});
	}

	private sealed class IsBetweenConstraint(
		string it,
		ExpectationGrammars grammars,
		DateTimeOffset? minimum,
		DateTimeOffset? maximum)
		: OrderingConstraint<DateTimeOffset?>(it, grammars, minimum is null || maximum is null),
			IValueConstraint<DateTimeOffset?>,
			IOptionsHolderSource<TimeTolerance>
	{
		private OptionsHolder<TimeTolerance>? _toleranceHolder;

		private TimeTolerance CurrentTolerance
			=> _toleranceHolder is null ? TimeTolerance.Default : _toleranceHolder.Value;

		/// <inheritdoc cref="IOptionsHolderSource{TOptions}.Holder" />
		OptionsHolder<TimeTolerance> IOptionsHolderSource<TimeTolerance>.Holder
			=> _toleranceHolder ??= new OptionsHolder<TimeTolerance>(TimeTolerance.Default);

		public ConstraintResult IsMetBy(DateTimeOffset? actual)
		{
			Actual = actual;
			if (actual is null || minimum is null || maximum is null)
			{
				Outcome = Outcome.FailureBothWays;
			}
			else
			{
				TimeSpan timeTolerance = CurrentTolerance.GetToleranceOrDefault();
				Outcome = minimum - actual.Value <= timeTolerance &&
				          actual.Value - maximum <= timeTolerance
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
			stringBuilder.Append(CurrentTolerance);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
			stringBuilder.AppendTimeDifferenceToRange(Actual?.UtcTicks, minimum?.UtcTicks, maximum?.UtcTicks);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not between ", "are not between "));
			Formatter.Format(stringBuilder, minimum);
			stringBuilder.Append(" and ");
			Formatter.Format(stringBuilder, maximum);
			stringBuilder.Append(CurrentTolerance);
		}
	}
}
