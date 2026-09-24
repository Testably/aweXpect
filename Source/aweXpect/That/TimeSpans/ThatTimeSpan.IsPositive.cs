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
	/// <remarks>
	///     <see cref="TimeSpan.Zero" /> is neither positive nor negative, so it fails
	///     <see cref="IsPositive(IThat{TimeSpan})" /> and <see cref="IsNegative(IThat{TimeSpan})" /> and satisfies
	///     <see cref="IsNotPositive(IThat{TimeSpan})" /> and <see cref="IsNotNegative(IThat{TimeSpan})" />.
	/// </remarks>
	public static AndOrResult<TimeSpan, IThat<TimeSpan>> IsPositive(this IThat<TimeSpan> subject)
		=> new(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsPositiveConstraint(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that the subject is not positive.
	/// </summary>
	/// <remarks>
	///     <see cref="TimeSpan.Zero" /> is neither positive nor negative, so it fails
	///     <see cref="IsPositive(IThat{TimeSpan})" /> and <see cref="IsNegative(IThat{TimeSpan})" /> and satisfies
	///     <see cref="IsNotPositive(IThat{TimeSpan})" /> and <see cref="IsNotNegative(IThat{TimeSpan})" />.
	/// </remarks>
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
			=> stringBuilder.Append(Grammars.Verb("is positive", "are positive"));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("is not positive", "are not positive"));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
