using System.Reflection;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Reflection.Helpers;
using aweXpect.Results;

namespace aweXpect.Reflection;

public static partial class ThatMethod
{
	/// <summary>
	///     Verifies that the <see cref="MethodInfo" /> is generic.
	/// </summary>
	public static AndOrResult<MethodInfo?, IThat<MethodInfo?>> IsGeneric(
		this IThat<MethodInfo?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsGenericConstraint(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that the <see cref="MethodInfo" /> is not generic.
	/// </summary>
	public static AndOrResult<MethodInfo?, IThat<MethodInfo?>> IsNotGeneric(
		this IThat<MethodInfo?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsGenericConstraint(it, grammars).Invert()),
			subject);

	private sealed class IsGenericConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithNotNullValue<MethodInfo?>(it, grammars),
			IValueConstraint<MethodInfo?>
	{
		public ConstraintResult IsMetBy(MethodInfo? actual)
		{
			Actual = actual;
			Outcome = actual?.IsGenericMethod == true ? Outcome.Success : Outcome.Failure;
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
