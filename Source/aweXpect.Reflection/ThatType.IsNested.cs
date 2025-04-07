using System;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Reflection.Extensions;
using aweXpect.Results;

namespace aweXpect.Reflection;

public static partial class ThatType
{
	/// <summary>
	///     Verifies that the <see cref="Type" /> is nested.
	/// </summary>
	public static AndOrResult<Type?, IThat<Type?>> IsNested(
		this IThat<Type?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsNestedConstraint(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that the <see cref="Type" /> is not nested.
	/// </summary>
	public static AndOrResult<Type?, IThat<Type?>> IsNotNested(
		this IThat<Type?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsNestedConstraint(it, grammars).Invert()),
			subject);

	private sealed class IsNestedConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithNotNullValue<Type?>(it, grammars),
			IValueConstraint<Type?>
	{
		public ConstraintResult IsMetBy(Type? actual)
		{
			Actual = actual;
			Outcome = actual?.IsNested == true ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is nested");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was non-nested ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is not nested");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was nested ");
			Formatter.Format(stringBuilder, Actual);
		}
	}
}
