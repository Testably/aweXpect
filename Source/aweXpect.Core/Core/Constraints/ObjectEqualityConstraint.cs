using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core.EvaluationContext;

namespace aweXpect.Core.Constraints;

/// <summary>
///     Verifies that the subject is equal to the <paramref name="expected" /> value, as the
///     <paramref name="options" /> compare them.
/// </summary>
/// <remarks>
///     Registered via <see cref="Extending.ExpectationBuilderExtensions" />. The expectation text shows the
///     <paramref name="expectedExpression" /> instead of the formatted <paramref name="expected" /> value, when it is
///     not <see langword="null" />.
/// </remarks>
internal sealed class ObjectEqualityConstraint<TSubject, TExpected>(
	string it,
	ExpectationGrammars grammars,
	TExpected expected,
	string? expectedExpression,
	OptionsHolder<ObjectMatchType<TSubject>> options)
	: ConstraintResult.WithEqualToValue<TSubject>(it, grammars, expected is null),
		IAsyncContextConstraint<TSubject>
{
	private ObjectMatcher<TSubject>? _matcher;
	private MatchResult _matchResult;

	private ObjectMatcher<TSubject> Matcher => _matcher ?? options.Value.ForEvaluation(default);

	public async ValueTask<ConstraintResult> IsMetBy(TSubject actual, IEvaluationContext context,
		CancellationToken cancellationToken)
	{
		Actual = actual;
		_matcher = options.Value.ForEvaluation(new MatchEnvironment(context, cancellationToken));
		_matchResult = await _matcher.CompareWithExplanation(actual, expected);
		Outcome = _matchResult.IsMatch ? Outcome.Success : Outcome.Failure;
		return this;
	}

	/// <inheritdoc />
	public override void AppendContexts(ResultContextCollector contexts)
		=> Matcher.AppendContexts(contexts);

	protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		=> stringBuilder.Append(options.Value.GetExpectation(
			expectedExpression ?? Formatter.Format(expected, FormattingOptions.Indented(indentation)), Grammars));

	protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		=> stringBuilder.Append(Matcher.GetFailure(It, Grammars, Actual, expected, _matchResult, indentation));

	protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		=> AppendNormalExpectation(stringBuilder, indentation);
}
