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
	///     Verifies that the <see cref="Type" /> is generic.
	/// </summary>
	public static AndOrResult<Type?, IThat<Type?>> IsGeneric(
		this IThat<Type?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsGenericConstraint(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that the <see cref="Type" /> is not generic.
	/// </summary>
	public static AndOrResult<Type?, IThat<Type?>> IsNotGeneric(
		this IThat<Type?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsGenericConstraint(it, grammars).Invert()),
			subject);

	private sealed class IsGenericConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithNotNullValue<Type?>(it, grammars),
			IValueConstraint<Type?>
	{
		public ConstraintResult IsMetBy(Type? actual)
		{
			Actual = actual;
			Outcome = actual?.IsGenericType == true ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is generic");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was non-generic ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is not generic");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was generic ");
			Formatter.Format(stringBuilder, Actual);
		}
	}
}
