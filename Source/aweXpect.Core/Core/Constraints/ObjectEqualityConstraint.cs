using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core.EvaluationContext;
using aweXpect.Options;

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
	ObjectEqualityOptions<TSubject> options)
	: ConstraintResult.WithEqualToValue<TSubject>(it, grammars, expected is null),
		IAsyncContextConstraint<TSubject>
{
	private ObjectEqualityOptions<TSubject>? _evaluationOptions;
	private MatchResult _matchResult;

	public async ValueTask<ConstraintResult> IsMetBy(TSubject actual, IEvaluationContext context,
		CancellationToken cancellationToken)
	{
		Actual = actual;
		_evaluationOptions = options.ForEvaluation(context, cancellationToken);
		_matchResult = await _evaluationOptions.MatchWithExplanation(actual, expected);
		Outcome = _matchResult.IsMatch ? Outcome.Success : Outcome.Failure;
		return this;
	}

	/// <inheritdoc />
	public override void AppendContexts(ResultContextCollector contexts)
		=> (_evaluationOptions ?? options).AppendContexts(contexts);

	protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		=> stringBuilder.Append(options.GetExpectation(
			expectedExpression ?? Formatter.Format(expected, FormattingOptions.Indented(indentation)), Grammars));

	protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		=> stringBuilder.Append((_evaluationOptions ?? options).GetExtendedFailure(It, Grammars, Actual, expected,
			_matchResult, indentation));

	protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		=> AppendNormalExpectation(stringBuilder, indentation);

	protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		=> AppendNormalResult(stringBuilder, indentation);
}
