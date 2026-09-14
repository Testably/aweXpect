using System;
using System.Globalization;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatEnum
{
	/// <summary>
	///     Verifies that the subject has the <paramref name="expected" /> value.
	/// </summary>
	public static AndOrResult<TEnum, IThat<TEnum>> HasValue<TEnum>(
		this IThat<TEnum> source,
		long? expected)
		where TEnum : struct, Enum
		=> new(source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new HasValueConstraint<TEnum>(it, grammars, expected)),
			source);

	/// <summary>
	///     Verifies that the subject does not have the <paramref name="unexpected" /> value.
	/// </summary>
	public static AndOrResult<TEnum, IThat<TEnum>> DoesNotHaveValue<TEnum>(
		this IThat<TEnum> source,
		long? unexpected)
		where TEnum : struct, Enum
		=> new(source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new HasValueConstraint<TEnum>(it, grammars, unexpected).Invert()),
			source);

	private sealed class HasValueConstraint<TEnum>(string it, ExpectationGrammars grammars, long? expectedValue)
		: ConstraintResult.WithNotNullValue<TEnum>(it, grammars),
			IValueConstraint<TEnum>
		where TEnum : struct, Enum
	{
		public ConstraintResult IsMetBy(TEnum actual)
		{
			Actual = actual;
			Outcome = Convert.ToInt64(actual, CultureInfo.InvariantCulture) == expectedValue
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("has value ");
			Formatter.Format(stringBuilder, expectedValue);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("does not have value ");
			Formatter.Format(stringBuilder, expectedValue);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
