using System;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.EvaluationContext;
using aweXpect.Helpers;
using aweXpect.Options;

namespace aweXpect;

/// <summary>
///     What a quantified expectation on the elements of a collection verifies for every item, e.g. that it is equal to
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
	///     Only a condition whose comparison evaluates expectations of its own returns another condition than itself,
	///     see <see cref="ObjectEqualityOptions{TSubject}.ForEvaluation(IEvaluationContext, CancellationToken)" />.
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
///     <paramref name="options" />.
/// </summary>
/// <remarks>
///     The <paramref name="options" /> also describe the comparison, so a <see cref="SubjectEqualityOptions{TItem,TMatch}" />
///     lets the comparer of a set subject decide and names it. Only the <paramref name="contextOptions" /> add contexts.
/// </remarks>
internal sealed class ElementEqualTo<TItem, TValue>(
	IOptionsEquality<TValue> options,
	TValue expected,
	ObjectEqualityOptions<TValue>? contextOptions = null)
	: ElementCondition<TItem>
{
	/// <inheritdoc />
	public override ValueTask<bool> IsMetBy(TItem item)
		=> options.AreConsideredEqual((TValue)(object?)item!, expected);

	/// <inheritdoc />
	public override ElementCondition<TItem> ForEvaluation(IEvaluationContext context,
		CancellationToken cancellationToken)
	{
		IOptionsEquality<TValue> evaluationOptions = options.ForEvaluation(context, cancellationToken);
		return ReferenceEquals(evaluationOptions, options)
			? this
			: new ElementEqualTo<TItem, TValue>(evaluationOptions, expected, contextOptions);
	}

	/// <inheritdoc />
	public override string GetExpectation(ExpectationGrammars grammars)
		=> ElementExpectations.IsEqualTo(grammars, Formatter.Format(expected), options);

	/// <inheritdoc />
	public override void UseComparerOf(object subject)
		=> (options as ISubjectComparing)?.UseComparerOf(subject);

	/// <inheritdoc />
	public override void AppendContexts(ResultContextCollector contexts)
		=> contextOptions?.AppendContexts(contexts);
}

/// <summary>
///     An item that is equal to the <paramref name="expected" /> string according to the <paramref name="options" />,
///     or to the comparer of a set subject, when the <paramref name="subjectOptions" /> let it decide.
/// </summary>
internal sealed class ElementEqualToString<TItem>(
	StringEqualityOptions options,
	string? expected,
	SubjectEqualityOptions<string?, string?>? subjectOptions = null)
	: ElementCondition<TItem>
{
	/// <inheritdoc />
	public override ValueTask<bool> IsMetBy(TItem item)
		=> subjectOptions is null
			? options.AreConsideredEqual((string?)(object?)item, expected)
			: subjectOptions.AreConsideredEqual((string?)(object?)item, expected);

	/// <inheritdoc />
	/// <remarks>
	///     The expected string is validated here, so that an unusable pattern is rejected whichever items the subject
	///     has.
	/// </remarks>
	public override ElementCondition<TItem> ForEvaluation(IEvaluationContext context,
		CancellationToken cancellationToken)
	{
		options.ValidateExpected(expected);
		return this;
	}

	/// <inheritdoc />
	public override string GetExpectation(ExpectationGrammars grammars)
		=> ElementExpectations.IsEqualToString(grammars, expected, options, subjectOptions);

	/// <inheritdoc />
	public override void UseComparerOf(object subject)
		=> subjectOptions?.UseComparerOf(subject);
}

/// <summary>
///     An item whose <typeparamref name="TValue" /> is equivalent to the <paramref name="expected" /> one according to
///     the <paramref name="options" />.
/// </summary>
internal sealed class ElementEquivalentTo<TItem, TValue, TExpected>(
	ObjectEqualityOptions<TValue> options,
	TExpected expected,
	string expectedExpression)
	: ElementCondition<TItem>
{
	/// <inheritdoc />
	public override ValueTask<bool> IsMetBy(TItem item)
		=> options.AreConsideredEqual((TValue)(object?)item!, expected);

	/// <inheritdoc />
	public override ElementCondition<TItem> ForEvaluation(IEvaluationContext context,
		CancellationToken cancellationToken)
	{
		ObjectEqualityOptions<TValue> evaluationOptions = options.ForEvaluation(context, cancellationToken);
		return ReferenceEquals(evaluationOptions, options)
			? this
			: new ElementEquivalentTo<TItem, TValue, TExpected>(evaluationOptions, expected, expectedExpression);
	}

	/// <inheritdoc />
	public override string GetExpectation(ExpectationGrammars grammars)
		=> ElementExpectations.IsEquivalentTo(grammars,
			expected is null ? Formatter.Format(expected) : expectedExpression.TrimCommonWhiteSpace());

	/// <inheritdoc />
	public override void AppendContexts(ResultContextCollector contexts)
		=> options.AppendContexts(contexts);
}
