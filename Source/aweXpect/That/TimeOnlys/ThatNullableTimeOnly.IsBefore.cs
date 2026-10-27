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
	///     Verifies that the subject is before the <paramref name="expected" /> value.
	/// </summary>
	/// <remarks>
	///     The times are compared linearly from midnight, so 00:01 is before 23:59, while
	///     <see cref="IsEqualTo(IThat{TimeOnly?}, TimeOnly?)" /> and <see cref="IsBetween(IThat{TimeOnly?}, TimeOnly?)" />
	///     treat the times as a clock face that wraps around midnight.
	/// </remarks>
	[GuaranteesNotNull]
	public static TimeToleranceResult<TimeOnly, IThat<TimeOnly?>> IsBefore(
		this IThat<TimeOnly?> subject,
		TimeOnly? expected)
	{
		return new TimeToleranceResult<TimeOnly, IThat<TimeOnly?>>(
			subject.Get().ExpectationBuilder.AddConstraint(expected,
				static (expected, it, grammars) =>
					new IsBeforeConstraint(it, grammars, expected),
				out IsBeforeConstraint constraint),
			subject,
			constraint);
	}

	/// <summary>
	///     Verifies that the subject is not before the <paramref name="unexpected" /> value.
	/// </summary>
	/// <remarks>
	///     The times are compared linearly from midnight, so 00:01 is before 23:59, while
	///     <see cref="IsEqualTo(IThat{TimeOnly?}, TimeOnly?)" /> and <see cref="IsBetween(IThat{TimeOnly?}, TimeOnly?)" />
	///     treat the times as a clock face that wraps around midnight.
	/// </remarks>
	[GuaranteesNotNull]
	public static TimeToleranceResult<TimeOnly, IThat<TimeOnly?>> IsNotBefore(
		this IThat<TimeOnly?> subject,
		TimeOnly? unexpected)
	{
		return new TimeToleranceResult<TimeOnly, IThat<TimeOnly?>>(
			subject.Get().ExpectationBuilder.AddConstraint(unexpected,
				static (unexpected, it, grammars) =>
					new IsBeforeConstraint(it, grammars, unexpected).Invert(),
				out IsBeforeConstraint constraint),
			subject,
			constraint);
	}

	private sealed class IsBeforeConstraint(
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
				Outcome = expected.Value.Ticks - actual.Value.Ticks > -timeTolerance.Ticks
					? Outcome.Success
					: Outcome.Failure;
			}

			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is before ", "are before "));
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
			stringBuilder.Append(Grammars.Verb("is not before ", "are not before "));
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(CurrentTolerance);
		}
	}
}
#endif
