using System;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatTimeSpan
{
	/// <summary>
	///     Verifies that the subject is positive.
	/// </summary>
	public static AndOrResult<TimeSpan, IThat<TimeSpan>> IsPositive(this IThat<TimeSpan> subject)
		=> new(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsPositiveConstraint(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that the subject is not positive.
	/// </summary>
	public static AndOrResult<TimeSpan, IThat<TimeSpan>> IsNotPositive(this IThat<TimeSpan> subject)
		=> new(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsPositiveConstraint(it, grammars).Invert()),
			subject);

	private sealed class IsPositiveConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithNotNullValue<TimeSpan>(it, grammars),
			IValueConstraint<TimeSpan>
	{
		public ConstraintResult IsMetBy(TimeSpan actual)
		{
			Actual = actual;
			Outcome = actual > TimeSpan.Zero ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is positive");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is not positive");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
