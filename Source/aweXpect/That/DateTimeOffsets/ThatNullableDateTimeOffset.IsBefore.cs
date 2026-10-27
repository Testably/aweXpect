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
	///     Verifies that the subject is before the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static TimeToleranceResult<DateTimeOffset, IThat<DateTimeOffset?>> IsBefore(
		this IThat<DateTimeOffset?> subject,
		DateTimeOffset? expected)
	{
		return new TimeToleranceResult<DateTimeOffset, IThat<DateTimeOffset?>>(
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
	[GuaranteesNotNull]
	public static TimeToleranceResult<DateTimeOffset, IThat<DateTimeOffset?>> IsNotBefore(
		this IThat<DateTimeOffset?> subject,
		DateTimeOffset? unexpected)
	{
		return new TimeToleranceResult<DateTimeOffset, IThat<DateTimeOffset?>>(
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
		DateTimeOffset? expected)
		: OrderingConstraint<DateTimeOffset?>(it, grammars, expected is null),
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
			if (actual is null || expected is null)
			{
				Outcome = Outcome.FailureBothWays;
			}
			else
			{
				TimeSpan timeTolerance = CurrentTolerance.GetToleranceOrDefault();
				Outcome = expected - actual.Value > timeTolerance.Negate() ? Outcome.Success : Outcome.Failure;
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
			stringBuilder.AppendTimeDifference((Actual - expected)?.Ticks);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not before ", "are not before "));
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(CurrentTolerance);
		}
	}
}
