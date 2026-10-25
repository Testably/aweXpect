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
	///     Verifies that the subject is on or after the <paramref name="expected" /> value.
	/// </summary>
	/// <remarks>
	///     Comparing a <see cref="DateTimeKind.Utc" /> with a <see cref="DateTimeKind.Local" /> value fails, for this
	///     expectation and its negation alike, while <see cref="DateTimeKind.Unspecified" /> can be compared with
	///     either kind.
	/// </remarks>
	[GuaranteesNotNull]
	public static TimeToleranceResult<DateTime, IThat<DateTime?>> IsOnOrAfter(
		this IThat<DateTime?> subject,
		DateTime? expected)
	{
		return new TimeToleranceResult<DateTime, IThat<DateTime?>>(
			subject.Get().ExpectationBuilder.AddConstraint(expected,
				static (expected, it, grammars) =>
					new IsOnOrAfterConstraint(it, grammars, expected),
				out IsOnOrAfterConstraint constraint),
			subject,
			constraint);
	}

	/// <summary>
	///     Verifies that the subject is not on or after the <paramref name="unexpected" /> value.
	/// </summary>
	/// <remarks>
	///     Comparing a <see cref="DateTimeKind.Utc" /> with a <see cref="DateTimeKind.Local" /> value fails, for this
	///     expectation and its negation alike, while <see cref="DateTimeKind.Unspecified" /> can be compared with
	///     either kind.
	/// </remarks>
	[GuaranteesNotNull]
	public static TimeToleranceResult<DateTime, IThat<DateTime?>> IsNotOnOrAfter(
		this IThat<DateTime?> subject,
		DateTime? unexpected)
	{
		return new TimeToleranceResult<DateTime, IThat<DateTime?>>(
			subject.Get().ExpectationBuilder.AddConstraint(unexpected,
				static (unexpected, it, grammars) =>
					new IsOnOrAfterConstraint(it, grammars, unexpected).Invert(),
				out IsOnOrAfterConstraint constraint),
			subject,
			constraint);
	}

	private sealed class IsOnOrAfterConstraint(
		string it,
		ExpectationGrammars grammars,
		DateTime? expected)
		: OrderingConstraint<DateTime?>(it, grammars, expected is null),
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
			if (actual is null || expected is null)
			{
				Outcome = Outcome.FailureBothWays;
			}
			else if (!EqualityHelpers.AreKindCompatible(actual.Value.Kind, expected.Value.Kind))
			{
				// Comparing ticks across incompatible kinds proves nothing, so the negated check fails as well.
				_incompatibleKind = expected.Value.Kind;
				IsIncomparable = true;
				Outcome = Outcome.FailureBothWays;
			}
			else
			{
				TimeSpan timeTolerance = CurrentTolerance.GetToleranceOrDefault();
				Outcome = expected - actual.Value <= timeTolerance ? Outcome.Success : Outcome.Failure;
			}

			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is on or after ", "are on or after "));
			Formatter.Format(stringBuilder, expected);
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
				stringBuilder.AppendTimeDifference((Actual - expected)?.Ticks);
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not on or after ", "are not on or after "));
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(CurrentTolerance);
		}
	}
}
