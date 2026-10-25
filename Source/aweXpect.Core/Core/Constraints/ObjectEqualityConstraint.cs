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
///     not <see langword="null" />. The match type rejects an unusable <paramref name="expected" /> value against the
///     <paramref name="parameterName" />.
/// </remarks>
internal sealed class ObjectEqualityConstraint<TSubject, TExpected>(
	string it,
	ExpectationGrammars grammars,
	TExpected expected,
	string? expectedExpression,
	ObjectMatchType<TSubject> options,
	string parameterName)
	: ConstraintResult.WithEqualToValue<TSubject>(it, grammars, expected is null),
		IAsyncContextConstraint<TSubject>,
		IOptionsHolderSource<ObjectMatchType<TSubject>>
{
	private OptionsHolder<ObjectMatchType<TSubject>>? _optionsHolder;

	private ObjectMatchType<TSubject> CurrentOptions => _optionsHolder is null ? options : _optionsHolder.Value;

	/// <inheritdoc cref="IOptionsHolderSource{TOptions}.Holder" />
	OptionsHolder<ObjectMatchType<TSubject>> IOptionsHolderSource<ObjectMatchType<TSubject>>.Holder
		=> _optionsHolder ??= new OptionsHolder<ObjectMatchType<TSubject>>(options);

	private ObjectMatcher<TSubject>? _matcher;
	private MatchResult _matchResult;

	/// <summary>
	///     Compares with the options in the <paramref name="options" /> holder, which the caller shares with its result.
	/// </summary>
	public ObjectEqualityConstraint(string it, ExpectationGrammars grammars, TExpected expected,
		string? expectedExpression, OptionsHolder<ObjectMatchType<TSubject>> options, string parameterName)
		: this(it, grammars, expected, expectedExpression, options.Value, parameterName)
	{
		_optionsHolder = options;
	}

	private ObjectMatcher<TSubject> Matcher => _matcher ?? CurrentOptions.ForEvaluation(default);

	public async ValueTask<ConstraintResult> IsMetBy(TSubject actual, IEvaluationContext context,
		CancellationToken cancellationToken)
	{
		Actual = actual;
		ObjectMatchType<TSubject> matchType = CurrentOptions;
		ValidateExpected(matchType);
		_matcher = matchType.ForEvaluation(new MatchEnvironment(context, cancellationToken));
		_matchResult = await _matcher.CompareWithExplanation(actual, expected);
		Outcome = _matchResult.IsMatch ? Outcome.Success : Outcome.Failure;
		return this;
	}

	/// <remarks>
	///     Only an expected value that the match type can receive is validated: <see langword="null" /> when the subject
	///     can be <see langword="null" />, and otherwise a <typeparamref name="TSubject" />, e.g. not the anonymous object
	///     of an equivalency.
	/// </remarks>
	private void ValidateExpected(ObjectMatchType<TSubject> matchType)
	{
		if (expected is TSubject typedExpected)
		{
			matchType.ValidateExpected(typedExpected, parameterName);
		}
		else if (expected is null && default(TSubject) is null)
		{
			matchType.ValidateExpected(default!, parameterName);
		}
	}

	/// <inheritdoc />
	public override void AppendContexts(ResultContextCollector contexts)
		=> Matcher.AppendContexts(contexts);

	protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		=> stringBuilder.Append(CurrentOptions.GetExpectation(
			expectedExpression ?? Formatter.Format(expected, FormattingOptions.Indented(indentation)), Grammars));

	protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		=> stringBuilder.Append(Matcher.GetFailure(It, Grammars, Actual, expected, _matchResult, indentation));

	protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		=> AppendNormalExpectation(stringBuilder, indentation);
}
