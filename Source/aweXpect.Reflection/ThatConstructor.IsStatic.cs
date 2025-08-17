using System.Reflection;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Reflection.Helpers;
using aweXpect.Results;

namespace aweXpect.Reflection;

public static partial class ThatConstructor
{
	/// <summary>
	///     Verifies that the <see cref="ConstructorInfo" /> is static.
	/// </summary>
	public static AndOrResult<ConstructorInfo?, IThat<ConstructorInfo?>> IsStatic(
		this IThat<ConstructorInfo?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsStaticConstraint(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that the <see cref="ConstructorInfo" /> is not static.
	/// </summary>
	public static AndOrResult<ConstructorInfo?, IThat<ConstructorInfo?>> IsNotStatic(
		this IThat<ConstructorInfo?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsStaticConstraint(it, grammars).Invert()),
			subject);

	private sealed class IsStaticConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithNotNullValue<ConstructorInfo?>(it, grammars),
			IValueConstraint<ConstructorInfo?>
	{
		public ConstraintResult IsMetBy(ConstructorInfo? actual)
		{
			Actual = actual;
			Outcome = actual?.IsStatic == true ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is static");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was non-static ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is not static");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was static ");
			Formatter.Format(stringBuilder, Actual);
		}
	}
}
