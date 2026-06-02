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
	///     Verifies that the <see cref="MethodInfo" /> overrides a base class method.
	/// </summary>
	public static AndOrResult<MethodInfo?, IThat<MethodInfo?>> Overrides(
		this IThat<MethodInfo?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new OverridesConstraint(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that the <see cref="MethodInfo" /> does not override a base class method.
	/// </summary>
	public static AndOrResult<MethodInfo?, IThat<MethodInfo?>> DoesNotOverride(
		this IThat<MethodInfo?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new OverridesConstraint(it, grammars).Invert()),
			subject);

	private sealed class OverridesConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithNotNullValue<MethodInfo?>(it, grammars),
			IValueConstraint<MethodInfo?>
	{
		public ConstraintResult IsMetBy(MethodInfo? actual)
		{
			Actual = actual;
			Outcome = actual?.IsOverride() == true ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("overrides a base method");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" did not override a base method ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("does not override a base method");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" did override a base method ");
			Formatter.Format(stringBuilder, Actual);
		}
	}
}
