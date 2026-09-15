using System;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatNullableEnum
{
	/// <summary>
	///     Verifies that the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	public static AndOrResult<TEnum?, IThat<TEnum?>> IsEqualTo<TEnum>(this IThat<TEnum?> subject,
		TEnum? expected)
		where TEnum : struct, Enum
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToConstraint<TEnum>(it, grammars, expected)),
			subject);

	/// <summary>
	///     Verifies that the subject is not equal to the <paramref name="unexpected" /> value.
	/// </summary>
	public static AndOrResult<TEnum?, IThat<TEnum?>> IsNotEqualTo<TEnum>(
		this IThat<TEnum?> subject,
		TEnum? unexpected)
		where TEnum : struct, Enum
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToConstraint<TEnum>(it, grammars, unexpected).Invert()),
			subject);

	private sealed class IsEqualToConstraint<TEnum>(string it, ExpectationGrammars grammars, TEnum? expected)
		: ConstraintResult.WithEqualToValue<TEnum?>(it, grammars, expected is null),
			IValueConstraint<TEnum?>
		where TEnum : struct, Enum
	{
		public ConstraintResult IsMetBy(TEnum? actual)
		{
			Actual = actual;
			Outcome = actual.Equals(expected) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is ");
			Formatter.Format(stringBuilder, expected);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is not ");
			Formatter.Format(stringBuilder, expected);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
