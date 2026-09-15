using System;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatVersion
{
	/// <summary>
	///     Verifies that the subject is less than the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<Version?, IThat<Version?>> IsLessThan(
		this IThat<Version?> subject,
		Version? expected)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsLessThanConstraint(it, grammars, expected)),
			subject);

	/// <summary>
	///     Verifies that the subject is not less than the <paramref name="unexpected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<Version?, IThat<Version?>> IsNotLessThan(
		this IThat<Version?> subject,
		Version? unexpected)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsLessThanConstraint(it, grammars, unexpected).Invert()),
			subject);

	private sealed class IsLessThanConstraint(
		string it,
		ExpectationGrammars grammars,
		Version? expected)
		: ConstraintResult.WithNotNullValue<Version?>(it, grammars),
			IValueConstraint<Version?>
	{
		public ConstraintResult IsMetBy(Version? actual)
		{
			Actual = actual;
			if (expected is null)
			{
				Outcome = IsNegated ? Outcome.Success : Outcome.Failure;
				return this;
			}

			Outcome = actual?.CompareTo(expected) < 0 ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is less than ");
			Formatter.Format(stringBuilder, expected);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is not less than ");
			Formatter.Format(stringBuilder, expected);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
