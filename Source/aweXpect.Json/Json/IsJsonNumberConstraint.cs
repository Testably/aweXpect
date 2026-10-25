using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;

namespace aweXpect.Json;

/// <summary>
///     Verifies that the subject is a JSON number, and that its value as <see langword="double" /> satisfies the
///     comparison of an operator on the result of <c>IsNumber()</c>, e.g. <c>GreaterThan(3)</c>.
/// </summary>
internal sealed class IsJsonNumberConstraint(
	string it,
	ExpectationGrammars grammars,
	JsonComparisonSlot<JsonNumberComparison> slot)
	: IsJsonValueKindConstraint<double>(it, grammars, JsonValueReaders.Number)
{
	protected override bool HasComparison => slot.Comparison is not null;

	protected override ValueTask<Outcome> Compare(double value, IEvaluationContext context,
		CancellationToken cancellationToken)
		=> new(slot.Comparison!.Evaluate(value));

	protected override void AppendComparison(StringBuilder stringBuilder)
	{
		JsonNumberComparison comparison = slot.Comparison!;
		if (comparison.IsNegated)
		{
			stringBuilder.Append("not ");
		}

		comparison.AppendExpectation(stringBuilder);
	}

	protected override void AppendComparedResult(StringBuilder stringBuilder, string? indentation)
		=> AppendActualValue(stringBuilder);
}
