using System;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatNullableTimeSpan
{
	/// <summary>
	///     Verifies that the subject is greater than or equal to the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static TimeToleranceResult<TimeSpan, IThat<TimeSpan?>> IsGreaterThanOrEqualTo(
		this IThat<TimeSpan?> subject,
		TimeSpan? expected)
	{
		return new TimeToleranceResult<TimeSpan, IThat<TimeSpan?>>(
			subject.Get().ExpectationBuilder.AddConstraint(expected,
				static (expected, it, grammars) =>
					new IsGreaterThanOrEqualToConstraint(it, grammars, expected),
				out IsGreaterThanOrEqualToConstraint constraint),
			subject,
			constraint);
	}

	/// <summary>
	///     Verifies that the subject is not greater than or equal to the <paramref name="unexpected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static TimeToleranceResult<TimeSpan, IThat<TimeSpan?>> IsNotGreaterThanOrEqualTo(
		this IThat<TimeSpan?> subject,
		TimeSpan? unexpected)
	{
		return new TimeToleranceResult<TimeSpan, IThat<TimeSpan?>>(
			subject.Get().ExpectationBuilder.AddConstraint(unexpected,
				static (unexpected, it, grammars) =>
					new IsGreaterThanOrEqualToConstraint(it, grammars, unexpected).Invert(),
				out IsGreaterThanOrEqualToConstraint constraint),
			subject,
			constraint);
	}

	private sealed class IsGreaterThanOrEqualToConstraint(
		string it,
		ExpectationGrammars grammars,
		TimeSpan? expected)
		: OrderingConstraint<TimeSpan?>(it, grammars, expected is null),
			IValueConstraint<TimeSpan?>,
			IOptionsHolderSource<TimeTolerance>
	{
		private OptionsHolder<TimeTolerance>? _toleranceHolder;

		private TimeTolerance CurrentTolerance
			=> _toleranceHolder is null ? TimeTolerance.Default : _toleranceHolder.Value;

		/// <inheritdoc cref="IOptionsHolderSource{TOptions}.Holder" />
		OptionsHolder<TimeTolerance> IOptionsHolderSource<TimeTolerance>.Holder
			=> _toleranceHolder ??= new OptionsHolder<TimeTolerance>(TimeTolerance.Default);

		public ConstraintResult IsMetBy(TimeSpan? actual)
		{
			Actual = actual;
			if (expected is null)
			{
				Outcome = Outcome.FailureBothWays;
				return this;
			}

			TimeSpan timeTolerance = CurrentTolerance.GetToleranceOrDefault();
			Outcome = actual.ShiftedTicks(timeTolerance) >= expected.Value.Ticks ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is greater than or equal to ", "are greater than or equal to "));
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
			stringBuilder.Append(Grammars.Verb("is not greater than or equal to ", "are not greater than or equal to "));
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(CurrentTolerance);
		}
	}
}
