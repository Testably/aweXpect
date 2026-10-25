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
	///     Verifies that the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	/// <remarks>
	///     The times are compared on the clock face, where the difference runs the shorter way around midnight, so
	///     23:59 and 00:01 are two minutes apart. The ordering expectations, like
	///     <see cref="IsAfter(IThat{TimeOnly?}, TimeOnly?)" />, compare the times linearly from midnight instead.
	/// </remarks>
	public static TimeToleranceResult<TimeOnly?, IThat<TimeOnly?>> IsEqualTo(this IThat<TimeOnly?> subject,
		TimeOnly? expected)
	{
		return new TimeToleranceResult<TimeOnly?, IThat<TimeOnly?>>(
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
	///     The times are compared on the clock face, where the difference runs the shorter way around midnight, so
	///     23:59 and 00:01 are two minutes apart. The ordering expectations, like
	///     <see cref="IsAfter(IThat{TimeOnly?}, TimeOnly?)" />, compare the times linearly from midnight instead.
	/// </remarks>
	public static TimeToleranceResult<TimeOnly?, IThat<TimeOnly?>> IsNotEqualTo(
		this IThat<TimeOnly?> subject,
		TimeOnly? unexpected)
	{
		return new TimeToleranceResult<TimeOnly?, IThat<TimeOnly?>>(
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
		TimeOnly? expected)
		: ConstraintResult.WithEqualToValue<TimeOnly?>(it, grammars, expected is null),
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
			if (actual is null && expected is null)
			{
				Outcome = Outcome.Success;
			}
			else if (actual is null || expected is null)
			{
				Outcome = Outcome.Failure;
			}
			else
			{
				TimeSpan timeTolerance = CurrentTolerance.GetToleranceOrDefault();
				Outcome = actual.Value.IsConsideredEqualTo(expected, timeTolerance)
					? Outcome.Success
					: Outcome.Failure;
			}

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
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
			stringBuilder.AppendCircularTimeDifference(Actual, expected);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not equal to ", "are not equal to "));
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(CurrentTolerance);
		}
	}
}
#endif
