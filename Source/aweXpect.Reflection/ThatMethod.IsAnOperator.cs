using System.Reflection;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Reflection;
using aweXpect.Reflection.Formatting;
using aweXpect.Reflection.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatMethod
{
	[GenerateExpectation("Is{Not}AnOperator", "is {not} an operator",
		Result = nameof(DescribeOperator),
		Summary = "Verifies that the <see cref=\"MethodInfo\" /> is an operator (e.g. <c>op_Addition</c>, <c>op_Equality</c>, …).",
		NegatedSummary = "Verifies that the <see cref=\"MethodInfo\" /> is not an operator (e.g. <c>op_Addition</c>, <c>op_Equality</c>, …).")]
	private static bool IsAnOperatorMethod(MethodInfo method) => method.IsOperator();

	private static string DescribeOperator(MethodInfo method)
		=> ResultTexts.Describe(method.IsOperator(), "was an operator", "was not an operator", method);

	/// <summary>
	///     Verifies that the <see cref="MethodInfo" /> is the specific <paramref name="operator" />
	///     (e.g. <see cref="Operator.Addition" /> matches <c>op_Addition</c>).
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<MethodInfo, IThat<MethodInfo?>> IsAnOperator(
		this IThat<MethodInfo?> subject,
		Operator @operator)
		=> new(subject.Get().ExpectationBuilder.AddConstraint(@operator,
				static (@operator, it, grammars)
				=> new IsTheOperatorConstraint(it, grammars, @operator)),
			subject);

	/// <summary>
	///     Verifies that the <see cref="MethodInfo" /> is not the specific <paramref name="operator" />
	///     (e.g. <see cref="Operator.Addition" /> matches <c>op_Addition</c>).
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<MethodInfo, IThat<MethodInfo?>> IsNotAnOperator(
		this IThat<MethodInfo?> subject,
		Operator @operator)
		=> new(subject.Get().ExpectationBuilder.AddConstraint(@operator,
				static (@operator, it, grammars)
				=> new IsTheOperatorConstraint(it, grammars, @operator).Invert()),
			subject);

	private sealed class IsTheOperatorConstraint(string it, ExpectationGrammars grammars, Operator @operator)
		: ConstraintResult.WithNotNullValue<MethodInfo?>(it, grammars),
			IValueConstraint<MethodInfo?>
	{
		private readonly string _name = OperatorNames.Display(@operator);

		public ConstraintResult IsMetBy(MethodInfo? actual)
		{
			Actual = actual;
			Outcome = actual.IsOperator(@operator) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is the operator ").Append(_name);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was not the operator ").Append(_name).Append(' ');
			Formatter.Format(stringBuilder, FormattableMember.From(Actual));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is not the operator ").Append(_name);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was the operator ").Append(_name).Append(' ');
			Formatter.Format(stringBuilder, FormattableMember.From(Actual));
		}
	}
}
