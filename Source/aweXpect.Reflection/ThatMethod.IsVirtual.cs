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
	///     Verifies that the <see cref="MethodInfo" /> is virtual.
	/// </summary>
	public static AndOrResult<MethodInfo?, IThat<MethodInfo?>> IsVirtual(
		this IThat<MethodInfo?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsVirtualConstraint(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that the <see cref="MethodInfo" /> is not virtual.
	/// </summary>
	public static AndOrResult<MethodInfo?, IThat<MethodInfo?>> IsNotVirtual(
		this IThat<MethodInfo?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsVirtualConstraint(it, grammars).Invert()),
			subject);

	private sealed class IsVirtualConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithNotNullValue<MethodInfo?>(it, grammars),
			IValueConstraint<MethodInfo?>
	{
		public ConstraintResult IsMetBy(MethodInfo? actual)
		{
			Actual = actual;
			Outcome = actual?.IsVirtual == true ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is virtual");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was non-virtual ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is not virtual");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was virtual ");
			Formatter.Format(stringBuilder, Actual);
		}
	}
}
