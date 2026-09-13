using System.Text;

namespace aweXpect.Core.Constraints;

internal sealed class IsExactlyOfTypeConstraint<TActual, TType>(string it, ExpectationGrammars grammars)
	: ConstraintResult.WithNotNullValue<TActual>(it, grammars),
		IValueConstraint<TActual>
{
	public ConstraintResult IsMetBy(TActual actual)
	{
		Actual = actual;
		Outcome = actual?.GetType() == typeof(TType) ? Outcome.Success : Outcome.Failure;
		return this;
	}

	protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
	{
		stringBuilder.Append("is exactly type ");
		Formatter.Format(stringBuilder, typeof(TType));
	}

	protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
	{
		stringBuilder.Append(It).Append(" was ");
		Formatter.Format(stringBuilder, Actual, FormattingOptions.Indented(indentation, true));
	}

	protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
	{
		stringBuilder.Append("is not exactly type ");
		Formatter.Format(stringBuilder, typeof(TType));
	}

	protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		=> AppendNormalResult(stringBuilder, indentation);
}
