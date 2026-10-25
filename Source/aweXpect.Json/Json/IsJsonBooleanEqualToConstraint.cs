using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.EvaluationContext;

namespace aweXpect.Json;

/// <summary>
///     Verifies that the subject is a JSON boolean that is equal to the <paramref name="expected" /> value.
/// </summary>
internal sealed class IsJsonBooleanEqualToConstraint(
	string it,
	ExpectationGrammars grammars,
	bool expected)
	: IsJsonValueEqualToConstraint<bool>(it, grammars, JsonValueReaders.Boolean)
{
	protected override ValueTask<bool> IsMatch(bool value, IEvaluationContext context,
		CancellationToken cancellationToken)
		=> new(value == expected);

	protected override void AppendComparison(StringBuilder stringBuilder)
		=> Formatter.Format(stringBuilder.Append("equal to "), expected);
}
