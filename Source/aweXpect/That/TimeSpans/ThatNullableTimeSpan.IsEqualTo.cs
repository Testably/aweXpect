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
	///     Verifies that the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	public static TimeToleranceResult<TimeSpan?, IThat<TimeSpan?>> IsEqualTo(
		this IThat<TimeSpan?> subject,
		TimeSpan? expected)
	{
		return new TimeToleranceResult<TimeSpan?, IThat<TimeSpan?>>(
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
	public static TimeToleranceResult<TimeSpan?, IThat<TimeSpan?>> IsNotEqualTo(
		this IThat<TimeSpan?> subject,
		TimeSpan? unexpected)
	{
		return new TimeToleranceResult<TimeSpan?, IThat<TimeSpan?>>(
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
		TimeSpan? expected)
		: ConstraintResult.WithEqualToValue<TimeSpan?>(it, grammars, expected is null),
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
			Outcome = (actual is null && expected is null) ||
			          IsWithinTolerance(CurrentTolerance.Tolerance, actual, expected)
				? Outcome.Success
				: Outcome.Failure;
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
			stringBuilder.AppendTimeDifference((decimal?)Actual?.Ticks - expected?.Ticks);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not equal to ", "are not equal to "));
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(CurrentTolerance);
		}
	}
}
