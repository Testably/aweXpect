using System;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Reflection.Helpers;
using aweXpect.Results;

namespace aweXpect.Reflection;

public static partial class ThatType
{
	/// <summary>
	///     Verifies that the <see cref="Type" /> is an enum.
	/// </summary>
	public static AndOrResult<Type?, IThat<Type?>> IsAnEnum(
		this IThat<Type?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsAnEnumConstraint(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that the <see cref="Type" /> is not an enum.
	/// </summary>
	public static AndOrResult<Type?, IThat<Type?>> IsNotAnEnum(
		this IThat<Type?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsAnEnumConstraint(it, grammars).Invert()),
			subject);

	private sealed class IsAnEnumConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithNotNullValue<Type?>(it, grammars),
			IValueConstraint<Type?>
	{
		public ConstraintResult IsMetBy(Type? actual)
		{
			Actual = actual;
			Outcome = actual?.IsEnum == true ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is an enum");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was ").Append(GetTypeNameOfType(Actual)).Append(' ');
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is not an enum");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
