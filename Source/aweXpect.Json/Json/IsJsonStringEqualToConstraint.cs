using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.EvaluationContext;
using aweXpect.Options;

namespace aweXpect.Json;

/// <summary>
///     Verifies that the subject is a JSON string that matches the <paramref name="expected" /> value with the
///     <paramref name="options" />.
/// </summary>
internal sealed class IsJsonStringEqualToConstraint(
	string it,
	ExpectationGrammars grammars,
	string expected,
	OptionsHolder<StringEqualityOptions> options)
	: IsJsonValueEqualToConstraint<string>(it, grammars, JsonValueReaders.String)
{
	private string? _actualValue;
	private StringEqualityOptions? _evaluationOptions;
	private MatchResult _matchResult;

	protected override async ValueTask<bool> IsMatch(string value, IEvaluationContext context,
		CancellationToken cancellationToken)
	{
		_actualValue = value;
		_evaluationOptions = options.Value.ForEvaluation(context, cancellationToken);
		_matchResult = await _evaluationOptions.MatchWithExplanation(value, expected, nameof(expected));
		return _matchResult.IsMatch;
	}

	public override void AppendContexts(ResultContextCollector contexts)
	{
		(_evaluationOptions ?? options.Value).AppendContexts(contexts);
		base.AppendContexts(contexts);
	}

	protected override void AppendComparison(StringBuilder stringBuilder)
		=> stringBuilder.Append(options.Value.GetExpectation(expected,
			Grammars & ~(ExpectationGrammars.Active | ExpectationGrammars.Negated)));

	protected override void AppendMismatch(StringBuilder stringBuilder)
		=> stringBuilder.Append((_evaluationOptions ?? options.Value).GetExtendedFailure(It, Grammars, _actualValue,
			expected, _matchResult));
}
