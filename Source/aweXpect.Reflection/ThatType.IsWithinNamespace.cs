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
	///     Verifies that the <see cref="Type" /> is within the <paramref name="expected" /> namespace
	///     (including sub-namespaces).
	/// </summary>
	public static AndOrResult<Type?, IThat<Type?>> IsWithinNamespace(
		this IThat<Type?> subject, string expected)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsWithinNamespaceConstraint(it, grammars, expected)),
			subject);

	/// <summary>
	///     Verifies that the <see cref="Type" /> is not within the <paramref name="expected" /> namespace
	///     (including sub-namespaces).
	/// </summary>
	public static AndOrResult<Type?, IThat<Type?>> IsNotWithinNamespace(
		this IThat<Type?> subject, string expected)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsWithinNamespaceConstraint(it, grammars, expected).Invert()),
			subject);

	private sealed class IsWithinNamespaceConstraint(
		string it,
		ExpectationGrammars grammars,
		string expected)
		: ConstraintResult.WithNotNullValue<Type?>(it, grammars),
			IValueConstraint<Type?>
	{
		public ConstraintResult IsMetBy(Type? actual)
		{
			Actual = actual;
			Outcome = actual.IsWithinNamespace(expected) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is within namespace ").Append(Formatter.Format(expected));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" was in namespace ").Append(Formatter.Format(Actual!.Namespace));

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is not within namespace ").Append(Formatter.Format(expected));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
