using System;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatNullableEnum
{
	/// <summary>
	///     Verifies that the subject has the <paramref name="expectedFlag" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<TEnum?, IThat<TEnum?>> HasFlag<TEnum>(
		this IThat<TEnum?> subject,
		TEnum? expectedFlag)
		where TEnum : struct, Enum
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new HasFlagConstraint<TEnum>(it, grammars, expectedFlag)),
			subject);

	/// <summary>
	///     Verifies that the subject does not have the <paramref name="unexpectedFlag" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<TEnum?, IThat<TEnum?>> DoesNotHaveFlag<TEnum>(
		this IThat<TEnum?> subject,
		TEnum? unexpectedFlag)
		where TEnum : struct, Enum
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new HasFlagConstraint<TEnum>(it, grammars, unexpectedFlag).Invert()),
			subject);

	private sealed class HasFlagConstraint<TEnum>(string it, ExpectationGrammars grammars, TEnum? expectedFlag)
		: ConstraintResult.WithNotNullValue<TEnum?>(it, grammars),
			IValueConstraint<TEnum?>
		where TEnum : struct, Enum
	{
		public ConstraintResult IsMetBy(TEnum? actual)
		{
			Actual = actual;
			Outcome = HasNullableFlag(actual, expectedFlag) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("has flag ");
			Formatter.Format(stringBuilder, expectedFlag);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("does not have flag ");
			Formatter.Format(stringBuilder, expectedFlag);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);

		private static bool HasNullableFlag(TEnum? actual, TEnum? expectedFlag)
			=> (actual == null && expectedFlag == null) ||
			   (actual != null && expectedFlag != null &&
			    actual.Value.HasFlag(expectedFlag));
	}
}
