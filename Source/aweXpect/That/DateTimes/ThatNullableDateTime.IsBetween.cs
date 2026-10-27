using System;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatNullableDateTime
{
	/// <summary>
	///     Verifies that the subject is between the <paramref name="minimum" />…
	/// </summary>
	/// <remarks>
	///     Both bounds are inclusive. A maximum with fewer ticks than the <paramref name="minimum" /> throws an
	///     <see cref="ArgumentOutOfRangeException" /> whatever their kinds, while a <see langword="null" /> bound, or
	///     one whose <see cref="DateTime.Kind" /> cannot be compared with the subject's, like
	///     <see cref="DateTimeKind.Utc" /> against <see cref="DateTimeKind.Local" />, fails the expectation as well as its
	///     negation.
	/// </remarks>
	[GuaranteesNotNull]
	public static BetweenResult<TimeToleranceResult<DateTime, IThat<DateTime?>>, DateTime?> IsBetween(
		this IThat<DateTime?> subject,
		DateTime? minimum)
	{
		return new BetweenResult<TimeToleranceResult<DateTime, IThat<DateTime?>>, DateTime?>(maximum =>
		{
			ThrowHelper.ThrowIfMaximumIsBelowMinimum(minimum, maximum);
			return new TimeToleranceResult<DateTime, IThat<DateTime?>>(
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
	///     Both bounds are inclusive. A maximum with fewer ticks than the <paramref name="minimum" /> throws an
	///     <see cref="ArgumentOutOfRangeException" /> whatever their kinds, while a <see langword="null" /> bound, or
	///     one whose <see cref="DateTime.Kind" /> cannot be compared with the subject's, like
	///     <see cref="DateTimeKind.Utc" /> against <see cref="DateTimeKind.Local" />, fails the expectation as well as its
	///     negation.
	/// </remarks>
	[GuaranteesNotNull]
	public static BetweenResult<TimeToleranceResult<DateTime, IThat<DateTime?>>, DateTime?> IsNotBetween(
		this IThat<DateTime?> subject,
		DateTime? minimum)
	{
		return new BetweenResult<TimeToleranceResult<DateTime, IThat<DateTime?>>, DateTime?>(maximum =>
		{
			ThrowHelper.ThrowIfMaximumIsBelowMinimum(minimum, maximum);
			return new TimeToleranceResult<DateTime, IThat<DateTime?>>(
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
		DateTime? minimum,
		DateTime? maximum)
		: OrderingConstraint<DateTime?>(it, grammars, minimum is null || maximum is null),
			IValueConstraint<DateTime?>,
			IOptionsHolderSource<TimeTolerance>
	{
		private OptionsHolder<TimeTolerance>? _toleranceHolder;

		private TimeTolerance CurrentTolerance
			=> _toleranceHolder is null ? TimeTolerance.Default : _toleranceHolder.Value;

		/// <inheritdoc cref="IOptionsHolderSource{TOptions}.Holder" />
		OptionsHolder<TimeTolerance> IOptionsHolderSource<TimeTolerance>.Holder
			=> _toleranceHolder ??= new OptionsHolder<TimeTolerance>(TimeTolerance.Default);

		private DateTimeKind? _incompatibleKind;

		public ConstraintResult IsMetBy(DateTime? actual)
		{
			Actual = actual;
			_incompatibleKind = null;
			if (actual is null || minimum is null || maximum is null)
			{
				Outcome = Outcome.FailureBothWays;
			}
			else if (!EqualityHelpers.AreKindCompatible(actual.Value.Kind, minimum.Value.Kind))
			{
				// Comparing ticks across incompatible kinds proves nothing, so the negated check fails as well.
				_incompatibleKind = minimum.Value.Kind;
				IsIncomparable = true;
				Outcome = Outcome.FailureBothWays;
			}
			else if (!EqualityHelpers.AreKindCompatible(actual.Value.Kind, maximum.Value.Kind))
			{
				_incompatibleKind = maximum.Value.Kind;
				IsIncomparable = true;
				Outcome = Outcome.FailureBothWays;
			}
			else
			{
				Outcome = IsWithinRange(actual.Value, minimum.Value, maximum.Value) ? Outcome.Success : Outcome.Failure;
			}

			return this;
		}

		private bool IsWithinRange(DateTime actual, DateTime minimum, DateTime maximum)
		{
			TimeSpan timeTolerance = CurrentTolerance.GetToleranceOrDefault();
			return minimum - actual <= timeTolerance && actual - maximum <= timeTolerance;
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
			if (_incompatibleKind is not null)
			{
				stringBuilder.Append(It).Append(" had kind ").Append(Actual?.Kind)
					.Append(", which cannot be compared with ").Append(_incompatibleKind);
			}
			else
			{
				stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
				Formatter.Format(stringBuilder, Actual);
				stringBuilder.AppendTimeDifferenceToRange(Actual?.Ticks, minimum?.Ticks, maximum?.Ticks);
			}
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
