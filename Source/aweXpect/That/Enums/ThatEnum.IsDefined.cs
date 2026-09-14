using System;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatEnum
{
	/// <summary>
	///     Verifies that the subject is defined inside the <typeparamref name="TEnum" />.
	/// </summary>
	public static AndOrResult<TEnum, IThat<TEnum>> IsDefined<TEnum>(
		this IThat<TEnum> source)
		where TEnum : struct, Enum
		=> new(source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsDefinedConstraint<TEnum>(it, grammars)),
			source);

	/// <summary>
	///     Verifies that the subject is not defined inside the <typeparamref name="TEnum" />.
	/// </summary>
	public static AndOrResult<TEnum, IThat<TEnum>> IsNotDefined<TEnum>(
		this IThat<TEnum> source)
		where TEnum : struct, Enum
		=> new(source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsDefinedConstraint<TEnum>(it, grammars).Invert()),
			source);

	private sealed class IsDefinedConstraint<TEnum>(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithNotNullValue<TEnum>(it, grammars),
			IValueConstraint<TEnum>
		where TEnum : struct, Enum
	{
		public ConstraintResult IsMetBy(TEnum actual)
		{
			Actual = actual;
			Outcome = Enum.IsDefined(typeof(TEnum), actual) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is defined");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is not defined");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
