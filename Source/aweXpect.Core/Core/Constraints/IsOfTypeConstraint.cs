using System.Text;

namespace aweXpect.Core.Constraints;

internal sealed class IsOfTypeConstraint<TActual, TType>(
	string it,
	ExpectationGrammars grammars)
	: ConstraintResult.WithNotNullValue<TActual>(it, grammars),
		IValueConstraint<TActual>
{
	public ConstraintResult IsMetBy(TActual actual)
	{
		Actual = actual;
		Outcome = actual is TType ? Outcome.Success : Outcome.Failure;
		return this;
	}

	public override void AppendContexts(ResultContextCollector contexts)
	{
		if (Actual is { } actual)
		{
			contexts.Add(new ResultContext.SyncCallback("Actual",
				() => Formatter.Format(actual, FormattingOptions.MultipleLines)));
		}
	}

	protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
	{
		stringBuilder.Append(Grammars.Verb("is of type ", "are of type "));
		Formatter.Format(stringBuilder, typeof(TType));
	}

	protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
	{
		stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
		Formatter.Format(stringBuilder, Actual!.GetType());
	}

	protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
	{
		stringBuilder.Append(Grammars.Verb("is not of type ", "are not of type "));
		Formatter.Format(stringBuilder, typeof(TType));
	}

	protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		=> AppendNormalResult(stringBuilder, indentation);
}
