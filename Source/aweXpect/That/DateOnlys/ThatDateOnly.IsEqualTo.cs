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
	///     Verifies that the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	public static TimeToleranceResult<DateOnly, IThat<DateOnly>> IsEqualTo(this IThat<DateOnly> subject,
		DateOnly? expected)
	{
		return new TimeToleranceResult<DateOnly, IThat<DateOnly>>(subject.Get().ExpectationBuilder
				.AddConstraint(expected,
					static (expected, it, grammars) =>
						new IsEqualToConstraint(it, grammars, expected),
					out IsEqualToConstraint constraint),
			subject,
			constraint);
	}

	/// <summary>
	///     Verifies that the subject is not equal to the <paramref name="unexpected" /> value.
	/// </summary>
	public static TimeToleranceResult<DateOnly, IThat<DateOnly>> IsNotEqualTo(
		this IThat<DateOnly> subject,
		DateOnly? unexpected)
	{
		return new TimeToleranceResult<DateOnly, IThat<DateOnly>>(
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
		DateOnly? expected)
		: ConstraintResult.WithEqualToValue<DateOnly>(it, grammars, expected is null),
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
				Outcome = Outcome.Failure;
			}
			else
			{
				TimeSpan timeTolerance = CurrentTolerance.GetToleranceOrDefault();
				Outcome = actual.IsConsideredEqualTo(expected, timeTolerance)
					? Outcome.Success
					: Outcome.Failure;
			}

			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is equal to ", "are equal to "));
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
			stringBuilder.Append(Grammars.Verb("is not equal to ", "are not equal to "));
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(CurrentTolerance.ToDayString());
		}
	}
}
#endif
