using System;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Customization;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatDateTime
{
	/// <summary>
	///     Verifies that the subject is between the <paramref name="minimum" />…
	/// </summary>
	/// <remarks>
	///     Both bounds are inclusive. A maximum below the <paramref name="minimum" /> throws an
	///     <see cref="ArgumentOutOfRangeException" />, while a <see langword="null" /> bound, or one whose
	///     <see cref="DateTime.Kind" /> cannot be compared with the subject's, like <see cref="DateTimeKind.Utc" />
	///     against <see cref="DateTimeKind.Local" />, fails the expectation as well as its negation.
	/// </remarks>
	public static BetweenResult<TimeToleranceResult<DateTime, IThat<DateTime>>, DateTime?> IsBetween(
		this IThat<DateTime> subject,
		DateTime? minimum)
	{
		TimeTolerance tolerance = new();
		return new BetweenResult<TimeToleranceResult<DateTime, IThat<DateTime>>, DateTime?>(maximum =>
		{
			ThrowHelper.ThrowIfMaximumIsBelowMinimum(minimum, maximum);
			return new TimeToleranceResult<DateTime, IThat<DateTime>>(
				subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new IsBetweenConstraint(it, grammars, minimum, maximum, tolerance)),
				subject,
				tolerance);
		});
	}

	/// <summary>
	///     Verifies that the subject is not between the <paramref name="minimum" />…
	/// </summary>
	/// <remarks>
	///     Both bounds are inclusive. A maximum below the <paramref name="minimum" /> throws an
	///     <see cref="ArgumentOutOfRangeException" />, while a <see langword="null" /> bound, or one whose
	///     <see cref="DateTime.Kind" /> cannot be compared with the subject's, like <see cref="DateTimeKind.Utc" />
	///     against <see cref="DateTimeKind.Local" />, fails the expectation as well as its negation.
	/// </remarks>
	public static BetweenResult<TimeToleranceResult<DateTime, IThat<DateTime>>, DateTime?> IsNotBetween(
		this IThat<DateTime> subject,
		DateTime? minimum)
	{
		TimeTolerance tolerance = new();
		return new BetweenResult<TimeToleranceResult<DateTime, IThat<DateTime>>, DateTime?>(maximum =>
		{
			ThrowHelper.ThrowIfMaximumIsBelowMinimum(minimum, maximum);
			return new TimeToleranceResult<DateTime, IThat<DateTime>>(
				subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new IsBetweenConstraint(it, grammars, minimum, maximum, tolerance).Invert()),
				subject,
				tolerance);
		});
	}

	private sealed class IsBetweenConstraint(
		string it,
		ExpectationGrammars grammars,
		DateTime? minimum,
		DateTime? maximum,
		TimeTolerance tolerance)
		: OrderingConstraint<DateTime>(it, grammars, minimum is null || maximum is null),
			IValueConstraint<DateTime>
	{
		private DateTimeKind? _incompatibleKind;

		public ConstraintResult IsMetBy(DateTime actual)
		{
			Actual = actual;
			if (minimum is null || maximum is null)
			{
				Outcome = Outcome.Failure;
			}
			else if (!EqualityHelpers.AreKindCompatible(actual.Kind, minimum.Value.Kind))
			{
				// Comparing ticks across incompatible kinds proves nothing, so the negated check fails as well.
				_incompatibleKind = minimum.Value.Kind;
				IsIncomparable = true;
			}
			else if (!EqualityHelpers.AreKindCompatible(actual.Kind, maximum.Value.Kind))
			{
				_incompatibleKind = maximum.Value.Kind;
				IsIncomparable = true;
			}
			else
			{
				Outcome = IsWithinRange(actual, minimum.Value, maximum.Value) ? Outcome.Success : Outcome.Failure;
			}

			return this;
		}

		private bool IsWithinRange(DateTime actual, DateTime minimum, DateTime maximum)
		{
			TimeSpan timeTolerance = tolerance.Tolerance
			                         ?? Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Get();
			return minimum - actual <= timeTolerance && actual - maximum <= timeTolerance;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is between ", "are between "));
			Formatter.Format(stringBuilder, minimum);
			stringBuilder.Append(" and ");
			Formatter.Format(stringBuilder, maximum);
			stringBuilder.Append(tolerance);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_incompatibleKind is not null)
			{
				stringBuilder.Append(It).Append(" had Kind ").Append(Actual.Kind)
					.Append(", which cannot be compared with ").Append(_incompatibleKind);
			}
			else
			{
				stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
				Formatter.Format(stringBuilder, Actual);
				stringBuilder.AppendTimeDifferenceToRange(Actual.Ticks, minimum?.Ticks, maximum?.Ticks);
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not between ", "are not between "));
			Formatter.Format(stringBuilder, minimum);
			stringBuilder.Append(" and ");
			Formatter.Format(stringBuilder, maximum);
			stringBuilder.Append(tolerance);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
