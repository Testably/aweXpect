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
	///     Verifies that the <see cref="Type" /> is abstract.
	/// </summary>
	public static AndOrResult<Type?, IThat<Type?>> IsAbstract(
		this IThat<Type?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsAbstractConstraint(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that the <see cref="Type" /> is not abstract.
	/// </summary>
	public static AndOrResult<Type?, IThat<Type?>> IsNotAbstract(
		this IThat<Type?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsAbstractConstraint(it, grammars).Invert()),
			subject);

	private sealed class IsAbstractConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithNotNullValue<Type?>(it, grammars),
			IValueConstraint<Type?>
	{
		public ConstraintResult IsMetBy(Type? actual)
		{
			Actual = actual;
			Outcome = actual?.IsAbstract == true ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is abstract");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was non-abstract ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is not abstract");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was abstract ");
			Formatter.Format(stringBuilder, Actual);
		}
	}
}
