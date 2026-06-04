using System.Reflection;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Reflection.Helpers;
using aweXpect.Results;

namespace aweXpect.Reflection;

public static partial class ThatAssembly
{
	/// <summary>
	///     Verifies that the <see cref="Assembly" /> is strong named.
	/// </summary>
	public static AndOrResult<Assembly?, IThat<Assembly?>> IsStrongNamed(
		this IThat<Assembly?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsStrongNamedConstraint(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that the <see cref="Assembly" /> is not strong named.
	/// </summary>
	public static AndOrResult<Assembly?, IThat<Assembly?>> IsNotStrongNamed(
		this IThat<Assembly?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsStrongNamedConstraint(it, grammars).Invert()),
			subject);

	private sealed class IsStrongNamedConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithNotNullValue<Assembly?>(it, grammars),
			IValueConstraint<Assembly?>
	{
		public ConstraintResult IsMetBy(Assembly? actual)
		{
			Actual = actual;
			Outcome = actual.IsStrongNamed() ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is strong named");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was not strong named ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is not strong named");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was strong named ");
			Formatter.Format(stringBuilder, Actual);
		}
	}
}
