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
	public static AndOrResult<Version?, IThat<Version?>> IsGreaterThan(
		this IThat<Version?> source,
		Version? expected)
		=> new(source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsGreaterThanConstraint(it, grammars, expected)),
			source);

	/// <summary>
	///     Verifies that the subject is not greater than the <paramref name="unexpected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<Version?, IThat<Version?>> IsNotGreaterThan(
		this IThat<Version?> source,
		Version? unexpected)
		=> new(source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsGreaterThanConstraint(it, grammars, unexpected).Invert()),
			source);

	private sealed class IsGreaterThanConstraint(
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

			Outcome = actual?.CompareTo(expected) > 0 ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is greater than ");
			Formatter.Format(stringBuilder, expected);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is not greater than ");
			Formatter.Format(stringBuilder, expected);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
