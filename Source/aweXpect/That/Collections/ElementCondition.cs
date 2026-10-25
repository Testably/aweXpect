using System;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Extending;
using aweXpect.Helpers;
using aweXpect.Options;

namespace aweXpect;

/// <summary>
///     What a quantified expectation on the items of a collection verifies for every item, e.g. that it is equal to
///     an expected value or satisfies a predicate.
/// </summary>
/// <remarks>
///     One object per expectation instead of separate delegates for the expectation text, the predicate and the
///     contexts, because every such expectation creates one.
/// </remarks>
internal abstract class ElementCondition<TItem>
{
	/// <summary>
	///     Whether the <paramref name="item" /> meets the condition.
	/// </summary>
	public abstract ValueTask<bool> IsMetBy(TItem item);

	/// <summary>
	///     Returns the condition to verify the items with during the evaluation in the <paramref name="context" />.
	/// </summary>
	/// <remarks>
	///     A condition that compares by options keeps the evaluator of the evaluation, see
	///     <see cref="IEqualityOptions{TSubject}.ForEvaluation(IEvaluationContext, CancellationToken)" />, or returns
	///     another condition for it.
	/// </remarks>
	public virtual ElementCondition<TItem> ForEvaluation(IEvaluationContext context,
		CancellationToken cancellationToken)
		=> this;

	/// <summary>
	///     The expectation on a single item for the <paramref name="grammars" />, e.g. "is equal to 1".
	/// </summary>
	public abstract string GetExpectation(ExpectationGrammars grammars);

	/// <summary>
	///     Lets the comparer of a set <paramref name="subject" /> decide, when the comparison allows it.
	/// </summary>
	public virtual void UseComparerOf(object subject)
	{
	}

	/// <summary>
	///     Adds the contexts of the options that decide the comparison.
	/// </summary>
	public virtual void AppendContexts(ResultContextCollector contexts)
	{
	}
}

/// <summary>
///     A condition that is verified synchronously.
/// </summary>
internal abstract class SynchronousElementCondition<TItem> : ElementCondition<TItem>
{
	/// <summary>
	///     Whether the <paramref name="item" /> meets the condition, without awaiting anything.
	/// </summary>
	public abstract bool IsMet(TItem item);

	/// <inheritdoc />
	public override ValueTask<bool> IsMetBy(TItem item)
		=> new(IsMet(item));
}

/// <summary>
///     An item that satisfies the <paramref name="predicate" /> on its <typeparamref name="TValue" />.
/// </summary>
/// <remarks>
///     A predicate on the items themselves is invoked directly, only the items of a struct collection, which are
///     enumerated as <see langword="object" />, are cast first.
/// </remarks>
internal sealed class ElementSatisfying<TItem, TValue>(Func<TValue, bool> predicate, string predicateExpression)
	: SynchronousElementCondition<TItem>
{
	private readonly Func<TItem, bool>? _itemPredicate = predicate as Func<TItem, bool>;

	/// <inheritdoc />
	public override bool IsMet(TItem item)
		=> _itemPredicate is null
			? UserCode.Invoke(static values => values.Predicate((TValue)(object?)values.Item!),
				(Predicate: predicate, Item: item), "the predicate")
			: UserCode.Invoke(_itemPredicate, item, "the predicate");

	/// <inheritdoc />
	public override string GetExpectation(ExpectationGrammars grammars)
		=> ElementExpectations.Satisfies(grammars, predicateExpression.TrimCommonWhiteSpace());
}

/// <summary>
///     An item of the type <typeparamref name="TType" />, or exactly of it.
/// </summary>
/// <remarks>
///     The condition holds no state, so one instance serves every expectation.
/// </remarks>
internal sealed class ElementOfType<TItem, TType> : SynchronousElementCondition<TItem>
{
	private readonly bool _exactly;
	private readonly Type _exactType = Nullable.GetUnderlyingType(typeof(TType)) ?? typeof(TType);

	private ElementOfType(bool exactly)
	{
		_exactly = exactly;
	}

	/// <summary>
	///     An item that is a <typeparamref name="TType" />.
	/// </summary>
	public static ElementOfType<TItem, TType> Instance { get; } = new(false);

	/// <summary>
	///     An item whose runtime type is exactly <typeparamref name="TType" />.
	/// </summary>
	public static ElementOfType<TItem, TType> ExactlyInstance { get; } = new(true);

	/// <inheritdoc />
	public override bool IsMet(TItem item)
		=> _exactly ? item?.GetType() == _exactType : item is TType;

	/// <inheritdoc />
	public override string GetExpectation(ExpectationGrammars grammars)
		=> _exactly
			? ElementExpectations.IsExactlyOfType(grammars, Formatter.Format(typeof(TType)))
			: ElementExpectations.IsOfType(grammars, Formatter.Format(typeof(TType)));
}

/// <summary>
///     An item of the <paramref name="type" />, or exactly of it.
/// </summary>
internal sealed class ElementOfType<TItem>(Type type, bool exactly) : SynchronousElementCondition<TItem>
{
	private readonly Type _exactType = Nullable.GetUnderlyingType(type) ?? type;

	/// <inheritdoc />
	public override bool IsMet(TItem item)
		=> exactly ? item?.GetType() == _exactType : type.IsInstanceOfType(item);

	/// <inheritdoc />
	public override string GetExpectation(ExpectationGrammars grammars)
		=> exactly
			? ElementExpectations.IsExactlyOfType(grammars, Formatter.Format(type))
			: ElementExpectations.IsOfType(grammars, Formatter.Format(type));
}

/// <summary>
///     An item whose <typeparamref name="TValue" /> is equal to the <paramref name="expected" /> one according to the
///     current <paramref name="options" />.
/// </summary>
/// <remarks>
///     The <paramref name="options" /> also describe the comparison, so a <see cref="SubjectEqualityOptions{TItem,TMatch}" />
///     lets the comparer of a set subject decide and names it. Only when it <paramref name="addsContexts" />, the
///     evaluator of the evaluation adds contexts.
/// </remarks>
internal sealed class ElementEqualTo<TItem, TValue>(
	IReadOnlyOptionsSlot<IEqualityOptions<TValue>> options,
	TValue expected,
	bool addsContexts = false)
	: ElementCondition<TItem>
{
	private IEqualityEvaluator<TValue>? _evaluator;

	private IEqualityEvaluator<TValue> Evaluator => _evaluator ?? options.WithoutEvaluation();

	/// <inheritdoc />
	public override ValueTask<bool> IsMetBy(TItem item)
		=> Evaluator.AreConsideredEqual((TValue)(object?)item!, expected);

	/// <inheritdoc />
	public override ElementCondition<TItem> ForEvaluation(IEvaluationContext context,
		CancellationToken cancellationToken)
	{
		_evaluator = options.ForEvaluation(context, cancellationToken);
		return this;
	}

	/// <inheritdoc />
	public override string GetExpectation(ExpectationGrammars grammars)
		=> ElementExpectations.IsEqualTo(grammars, Formatter.Format(expected), options);

	/// <inheritdoc />
	public override void UseComparerOf(object subject)
		=> (options as ISubjectComparing)?.UseComparerOf(subject);

	/// <inheritdoc />
	public override void AppendContexts(ResultContextCollector contexts)
	{
		if (addsContexts)
		{
			Evaluator.AppendContexts(contexts);
		}
	}
}

/// <summary>
///     An item that is equal to the <paramref name="expected" /> string according to the current options in the
///     <paramref name="options" /> slot, or to the comparer of a set subject, when the <paramref name="subjectOptions" />
///     let it decide.
/// </summary>
/// <remarks>
///     An unusable pattern is reported against the parameter <c>expected</c>. The options of the evaluation are kept,
///     so that the item compares and explains with them; before an evaluation, the current options describe the item.
/// </remarks>
internal sealed class ElementEqualToString<TItem>(
	IReadOnlyOptionsSlot<StringEqualityOptions> options,
	string? expected,
	SubjectEqualityOptions<string?, string?>? subjectOptions = null)
	: ElementCondition<TItem>
{
	private const string ParameterName = "expected";
	private StringEqualityOptions? _evaluationOptions;

	private StringEqualityOptions Options => _evaluationOptions ?? options.Value;

	/// <inheritdoc />
	public override ValueTask<bool> IsMetBy(TItem item)
		=> subjectOptions is { Comparer: not null, }
			? subjectOptions.AreConsideredEqual((string?)(object?)item, expected)
			: Options.AreConsideredEqual((string?)(object?)item, expected, ParameterName);

	/// <inheritdoc />
	/// <remarks>
	///     The expected string is validated here, so that an unusable pattern is rejected whichever items the subject
	///     has.
	/// </remarks>
	public override ElementCondition<TItem> ForEvaluation(IEvaluationContext context,
		CancellationToken cancellationToken)
	{
		StringEqualityOptions currentOptions = options.Value;
		currentOptions.ValidateExpected(expected, ParameterName);
		_evaluationOptions = currentOptions.ForEvaluation(context, cancellationToken);
		return this;
	}

	/// <inheritdoc />
	public override string GetExpectation(ExpectationGrammars grammars)
		=> ElementExpectations.IsEqualToString(grammars, expected, Options, subjectOptions);

	/// <inheritdoc />
	public override void UseComparerOf(object subject)
		=> subjectOptions?.UseComparerOf(subject);

	/// <inheritdoc />
	public override void AppendContexts(ResultContextCollector contexts)
		=> Options.AppendContexts(contexts);
}

/// <summary>
///     An item whose <typeparamref name="TValue" /> is equivalent to the <paramref name="expected" /> one according to
///     the <paramref name="matchType" />.
/// </summary>
internal sealed class ElementEquivalentTo<TItem, TValue, TExpected>(
	ObjectMatchType<TValue> matchType,
	TExpected expected,
	string expectedExpression)
	: ElementCondition<TItem>
{
	private ObjectMatcher<TValue>? _matcher;

	private IEqualityEvaluator<TValue> Matcher => _matcher ?? matchType.ForEvaluation(default);

	/// <inheritdoc />
	public override ValueTask<bool> IsMetBy(TItem item)
		=> Matcher.AreConsideredEqual((TValue)(object?)item!, expected);

	/// <inheritdoc />
	public override ElementCondition<TItem> ForEvaluation(IEvaluationContext context,
		CancellationToken cancellationToken)
	{
		_matcher = matchType.ForEvaluation(new MatchEnvironment(context, cancellationToken));
		return this;
	}

	/// <inheritdoc />
	public override string GetExpectation(ExpectationGrammars grammars)
		=> ElementExpectations.IsEquivalentTo(grammars,
			expected is null ? Formatter.Format(expected) : expectedExpression.TrimCommonWhiteSpace());

	/// <inheritdoc />
	public override void AppendContexts(ResultContextCollector contexts)
		=> Matcher.AppendContexts(contexts);
}
