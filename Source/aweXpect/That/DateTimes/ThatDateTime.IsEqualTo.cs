using System;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatDateTime
{
	/// <summary>
	///     Verifies that the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	/// <remarks>
	///     A <see cref="DateTimeKind.Utc" /> and a <see cref="DateTimeKind.Local" /> value cannot be compared and are
	///     therefore never equal, while <see cref="DateTimeKind.Unspecified" /> can be compared with either kind.
	/// </remarks>
	public static TimeToleranceResult<DateTime, IThat<DateTime>> IsEqualTo(
		this IThat<DateTime> subject,
		DateTime? expected)
	{
		return new TimeToleranceResult<DateTime, IThat<DateTime>>(
			subject.Get().ExpectationBuilder.AddConstraint(expected,
				static (expected, it, grammars) =>
					new IsEqualToConstraint(it, grammars, expected),
				out IsEqualToConstraint constraint),
			subject,
			constraint);
	}

	/// <summary>
	///     Verifies that the subject is not equal to the <paramref name="unexpected" /> value.
	/// </summary>
	/// <remarks>
	///     A <see cref="DateTimeKind.Utc" /> and a <see cref="DateTimeKind.Local" /> value cannot be compared and are
	///     therefore never equal, while <see cref="DateTimeKind.Unspecified" /> can be compared with either kind.
	/// </remarks>
	public static TimeToleranceResult<DateTime, IThat<DateTime>> IsNotEqualTo(
		this IThat<DateTime> subject,
		DateTime? unexpected)
	{
		return new TimeToleranceResult<DateTime, IThat<DateTime>>(
			subject.Get().ExpectationBuilder.AddConstraint(unexpected,
				static (unexpected, it, grammars) =>
					new IsEqualToConstraint(it, grammars, unexpected).Invert(),
				out IsEqualToConstraint constraint),
			subject,
			constraint);
	}

	private sealed class IsEqualToConstraint(
		string it,
		ExpectationGrammars grammars,
		DateTime? expected)
		: ConstraintResult.WithEqualToValue<DateTime>(it, grammars, expected is null),
			IValueConstraint<DateTime>,
			IOptionsHolderSource<TimeTolerance>
	{
		private OptionsHolder<TimeTolerance>? _toleranceHolder;

		private TimeTolerance CurrentTolerance
			=> _toleranceHolder is null ? TimeTolerance.Default : _toleranceHolder.Value;

		/// <inheritdoc cref="IOptionsHolderSource{TOptions}.Holder" />
		OptionsHolder<TimeTolerance> IOptionsHolderSource<TimeTolerance>.Holder
			=> _toleranceHolder ??= new OptionsHolder<TimeTolerance>(TimeTolerance.Default);

		private DateTimeKind? _incompatibleKind;

		public ConstraintResult IsMetBy(DateTime actual)
		{
			Actual = actual;

			TimeSpan timeTolerance = CurrentTolerance.GetToleranceOrDefault();
			Outcome = actual.IsConsideredEqualTo(expected, timeTolerance, out bool hasKindDifference)
				? Outcome.Success
				: Outcome.Failure;
			_incompatibleKind = hasKindDifference ? expected?.Kind : null;

			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is equal to ", "are equal to "));
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(CurrentTolerance);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_incompatibleKind is not null)
			{
				stringBuilder.Append(It).Append(" had kind ").Append(Actual.Kind)
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
			stringBuilder.Append(Grammars.Verb("is not equal to ", "are not equal to "));
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(CurrentTolerance);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
			stringBuilder.AppendTimeDifference((Actual - expected)?.Ticks);
		}
	}
}
