using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.EvaluationContext;
using aweXpect.Options;

namespace aweXpect.Json;

/// <summary>
///     Verifies that the subject is a JSON string that matches the <paramref name="expected" /> value.
/// </summary>
internal sealed class IsJsonStringEqualToConstraint(
	string it,
	ExpectationGrammars grammars,
	string expected)
	: IsJsonValueEqualToConstraint<string>(it, grammars, JsonValueReaders.String),
		IOptionsHolderSource<StringEqualityOptions>
{
	private OptionsHolder<StringEqualityOptions>? _optionsHolder;

	/// <summary>
	///     Matches with the options in the <paramref name="options" /> holder, which the caller shares with its result.
	/// </summary>
	public IsJsonStringEqualToConstraint(string it, ExpectationGrammars grammars, string expected,
		OptionsHolder<StringEqualityOptions> options)
		: this(it, grammars, expected)
	{
		_optionsHolder = options;
	}

	private StringEqualityOptions CurrentOptions
		=> _optionsHolder is null ? StringEqualityOptions.Default : _optionsHolder.Value;

	/// <inheritdoc cref="IOptionsHolderSource{TOptions}.Holder" />
	OptionsHolder<StringEqualityOptions> IOptionsHolderSource<StringEqualityOptions>.Holder
		=> _optionsHolder ??= new OptionsHolder<StringEqualityOptions>(StringEqualityOptions.Default);

	private string? _actualValue;
	private StringEqualityOptions? _evaluationOptions;
	private MatchResult _matchResult;

	protected override async ValueTask<bool> IsMatch(string value, IEvaluationContext context,
		CancellationToken cancellationToken)
	{
		_actualValue = value;
		_evaluationOptions = CurrentOptions.ForEvaluation(context, cancellationToken);
		_matchResult = await _evaluationOptions.MatchWithExplanation(value, expected, nameof(expected));
		return _matchResult.IsMatch;
	}

	public override void AppendContexts(ResultContextCollector contexts)
	{
		(_evaluationOptions ?? CurrentOptions).AppendContexts(contexts);
		base.AppendContexts(contexts);
	}

	protected override void AppendComparison(StringBuilder stringBuilder)
		=> stringBuilder.Append(CurrentOptions.GetExpectation(expected,
			Grammars & ~(ExpectationGrammars.Active | ExpectationGrammars.Negated)));

	protected override void AppendMismatch(StringBuilder stringBuilder)
		=> stringBuilder.Append((_evaluationOptions ?? CurrentOptions).GetExtendedFailure(It, Grammars, _actualValue,
			expected, _matchResult));
}
