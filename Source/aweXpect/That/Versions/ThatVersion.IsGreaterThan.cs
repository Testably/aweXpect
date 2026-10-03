using System;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatVersion
{
	/// <summary>
	///     Verifies that the subject is greater than the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<Version, IThat<Version?>> IsGreaterThan(
		this IThat<Version?> subject,
		Version? expected)
		=> new(subject.Get().ExpectationBuilder.AddConstraint(expected, static (expectedValue, it, grammars) =>
				new IsGreaterThanConstraint(it, grammars, expectedValue)),
			subject);

	/// <summary>
	///     Verifies that the subject is not greater than the <paramref name="unexpected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<Version, IThat<Version?>> IsNotGreaterThan(
		this IThat<Version?> subject,
		Version? unexpected)
		=> new(subject.Get().ExpectationBuilder.AddConstraint(unexpected, static (unexpectedValue, it, grammars) =>
				new IsGreaterThanConstraint(it, grammars, unexpectedValue).Invert()),
			subject);

	private sealed class IsGreaterThanConstraint(
		string it,
		ExpectationGrammars grammars,
		Version? expected)
		: OrderingConstraint<Version?>(it, grammars, expected is null),
			IValueConstraint<Version?>
	{
		public ConstraintResult IsMetBy(Version? actual)
		{
			Actual = actual;
			if (expected is null)
			{
				Outcome = Outcome.FailureBothWays;
				return this;
			}

			Outcome = actual?.CompareTo(expected) > 0 ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is greater than ", "are greater than "));
			Formatter.Format(stringBuilder, expected);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not greater than ", "are not greater than "));
			Formatter.Format(stringBuilder, expected);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
