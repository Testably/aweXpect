using System.Text;
using aweXpect.Core.Helpers;

namespace aweXpect.Core.Constraints;

internal sealed class IsExactlyOfTypeConstraint<TActual, TType>(
	string it,
	ExpectationGrammars grammars)
	: ConstraintResult.WithNotNullValue<TActual>(it, grammars),
		IValueConstraint<TActual>
{
	public ConstraintResult IsMetBy(TActual actual)
	{
		Actual = actual;
		Outcome = actual?.GetType() == typeof(TType) ? Outcome.Success : Outcome.Failure;
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
		stringBuilder.Append(Grammars.Verb("is exactly of type ", "are exactly of type "));
		Formatter.Format(stringBuilder, typeof(TType));
	}

	protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
	{
		stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
		Formatter.Format(stringBuilder, Actual!.GetType());
	}

	protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
	{
		stringBuilder.Append(Grammars.Verb("is not exactly of type ", "are not exactly of type "));
		Formatter.Format(stringBuilder, typeof(TType));
	}

	protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		=> AppendNormalResult(stringBuilder, indentation);
}
