using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Results;

namespace aweXpect.Json;

/// <summary>
///     Verifies that the subject is a JSON string, and that its value satisfies the comparison of an operator on the
///     result of <c>IsString()</c>, e.g. <c>StartingWith("Let")</c>.
/// </summary>
internal sealed class IsJsonStringConstraint(
	string it,
	ExpectationGrammars grammars,
	JsonComparisonSlot<StringPropertyComparison> slot)
	: IsJsonValueKindConstraint<string>(it, grammars, JsonValueReaders.String)
{
	private StringPropertyOutcome _outcome;

	protected override bool HasComparison => slot.Comparison is not null;

	protected override void ValidateComparison() => slot.Comparison?.ValidateExpected();

	protected override async ValueTask<Outcome> Compare(string value, IEvaluationContext context,
		CancellationToken cancellationToken)
	{
		_outcome = await slot.Comparison!.Evaluate(value, context, cancellationToken);
		return _outcome.Outcome;
	}

	public override void AppendContexts(ResultContextCollector contexts)
	{
		if (IsCompared)
		{
			_outcome.AppendContexts(contexts);
		}
	}

	protected override void AppendComparison(StringBuilder stringBuilder)
		=> slot.Comparison!.AppendExpectation(stringBuilder,
			slot.Comparison.IsNegated ? ExpectationGrammars.Negated : ExpectationGrammars.None);

	protected override void AppendComparedResult(StringBuilder stringBuilder, string? indentation)
		=> _outcome.AppendResult(stringBuilder, It, slot.Comparison!.IsNegated ? Grammars.Negate() : Grammars,
			indentation);
}
