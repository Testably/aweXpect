#if NET8_0_OR_GREATER
using System;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatNullableTimeOnly
{
	/// <summary>
	///     Verifies that the subject is after the <paramref name="expected" /> value.
	/// </summary>
	/// <remarks>
	///     The times are compared linearly from midnight, so 00:01 is before 23:59, while
	///     <see cref="IsEqualTo(IThat{TimeOnly?}, TimeOnly?)" /> and <see cref="IsBetween(IThat{TimeOnly?}, TimeOnly?)" />
	///     treat the times as a clock face that wraps around midnight.
	/// </remarks>
	[GuaranteesNotNull]
	public static TimeToleranceResult<TimeOnly, IThat<TimeOnly?>> IsAfter(
		this IThat<TimeOnly?> subject,
		TimeOnly? expected)
	{
		return new TimeToleranceResult<TimeOnly, IThat<TimeOnly?>>(
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
	/// <remarks>
	///     The times are compared linearly from midnight, so 00:01 is before 23:59, while
	///     <see cref="IsEqualTo(IThat{TimeOnly?}, TimeOnly?)" /> and <see cref="IsBetween(IThat{TimeOnly?}, TimeOnly?)" />
	///     treat the times as a clock face that wraps around midnight.
	/// </remarks>
	[GuaranteesNotNull]
	public static TimeToleranceResult<TimeOnly, IThat<TimeOnly?>> IsNotAfter(
		this IThat<TimeOnly?> subject,
		TimeOnly? unexpected)
	{
		return new TimeToleranceResult<TimeOnly, IThat<TimeOnly?>>(
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
		TimeOnly? expected)
		: OrderingConstraint<TimeOnly?>(it, grammars, expected is null),
			IValueConstraint<TimeOnly?>,
			IOptionsHolderSource<TimeTolerance>
	{
		private OptionsHolder<TimeTolerance>? _toleranceHolder;

		private TimeTolerance CurrentTolerance
			=> _toleranceHolder is null ? TimeTolerance.Default : _toleranceHolder.Value;

		/// <inheritdoc cref="IOptionsHolderSource{TOptions}.Holder" />
		OptionsHolder<TimeTolerance> IOptionsHolderSource<TimeTolerance>.Holder
			=> _toleranceHolder ??= new OptionsHolder<TimeTolerance>(TimeTolerance.Default);

		public ConstraintResult IsMetBy(TimeOnly? actual)
		{
			Actual = actual;
			if (actual is null || expected is null)
			{
				Outcome = Outcome.FailureBothWays;
			}
			else
			{
				TimeSpan timeTolerance = CurrentTolerance.GetToleranceOrDefault();
				Outcome = expected.Value.Ticks - actual.Value.Ticks < timeTolerance.Ticks
					? Outcome.Success
					: Outcome.Failure;
			}

			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is after ", "are after "));
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(CurrentTolerance);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
			stringBuilder.AppendTimeDifference((decimal?)Actual?.Ticks - expected?.Ticks);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not after ", "are not after "));
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(CurrentTolerance);
		}
	}
}
#endif
