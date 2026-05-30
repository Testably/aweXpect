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
	///     Verifies that the <see cref="ConstructorInfo" /> has the <paramref name="expected" /> number of parameters.
	/// </summary>
	public static AndOrResult<ConstructorInfo?, IThat<ConstructorInfo?>> HasParameterCount(
		this IThat<ConstructorInfo?> subject, int expected)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new HasParameterCountConstraint(it, grammars, expected)),
			subject);

	private static string ParameterCountDescription(int count)
		=> count switch
		{
			0 => "no parameters",
			1 => "one parameter",
			_ => $"{count} parameters",
		};

	private sealed class HasParameterCountConstraint(string it, ExpectationGrammars grammars, int expected)
		: ConstraintResult.WithNotNullValue<ConstructorInfo?>(it, grammars),
			IValueConstraint<ConstructorInfo?>
	{
		public ConstraintResult IsMetBy(ConstructorInfo? actual)
		{
			Actual = actual;
			Outcome = actual?.GetParameters().Length == expected ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("has ").Append(ParameterCountDescription(expected));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" had ")
				.Append(ParameterCountDescription(Actual?.GetParameters().Length ?? 0));

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("does not have ").Append(ParameterCountDescription(expected));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" did");
	}
}
