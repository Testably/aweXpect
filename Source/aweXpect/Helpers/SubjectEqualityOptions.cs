using System.Threading.Tasks;
using aweXpect.Core;

namespace aweXpect.Helpers;

/// <summary>
///     Lets the comparer of a collection subject decide how its items are compared.
/// </summary>
/// <remarks>
///     The constraints call it directly instead of receiving a delegate for it, because every expectation would
///     otherwise allocate that delegate.
/// </remarks>
internal interface ISubjectComparing
{
	/// <summary>
	///     Lets the comparer of the <paramref name="subject" /> decide, when it has one and the options do not change the
	///     comparison.
	/// </summary>
	/// <returns>
	///     <see langword="true" />, when the comparer of the <paramref name="subject" /> decides.
	/// </returns>
	bool UseComparerOf(object? subject);
}

/// <summary>
///     Equality options that let the comparer of a collection subject decide, as long as the options do not change the
///     comparison.
/// </summary>
/// <remarks>
///     The subject is only known during the evaluation, so <see cref="UseComparerOf" /> has to be called before
///     comparing, and the text of the options names the comparer only afterwards.<br />
///     It offers the current options of the wrapped holder as a covariant <see cref="IReadOnlyOptionsHolder{TOptions}" />,
///     so that a failure message of <c>aweXpect.Core</c> can describe the expected items by them.
/// </remarks>
internal sealed class SubjectEqualityOptions<TItem, TMatch>
	: IEqualityEvaluator<TMatch>, IReadOnlyOptionsHolder<IEqualityOptions<TMatch>>, ISubjectComparing,
		IWrapsEqualityEvaluator<TMatch>
{
	private readonly bool _canUseSubjectComparer;
	private readonly IReadOnlyOptionsHolder<IEqualityOptions<TMatch>> _options;
	private IEqualityEvaluator<TMatch>? _evaluator;

	/// <summary>
	///     Lets the comparer of the subject decide, when <paramref name="canUseSubjectComparer" /> and the comparison of
	///     the <paramref name="options" /> was not changed when the subject is evaluated.
	/// </summary>
	public SubjectEqualityOptions(IReadOnlyOptionsHolder<IEqualityOptions<TMatch>> options,
		bool canUseSubjectComparer = true)
	{
		_options = options;
		_canUseSubjectComparer = canUseSubjectComparer;
	}

	/// <summary>
	///     Lets the <paramref name="comparer" /> decide, which was already read from the subject, instead of the
	///     <paramref name="evaluator" /> of the evaluation.
	/// </summary>
	public SubjectEqualityOptions(IReadOnlyOptionsHolder<IEqualityOptions<TMatch>> options,
		IEqualityEvaluator<TMatch> evaluator, SubjectComparer<TItem> comparer)
	{
		_options = options;
		_evaluator = evaluator;
		Comparer = comparer;
	}

	/// <summary>
	///     The comparer of the subject that decides, or <see langword="null" /> when the options decide.
	/// </summary>
	public SubjectComparer<TItem>? Comparer { get; private set; }

	/// <summary>
	///     The evaluator of the evaluation, which decides whenever the comparer of the subject does not.
	/// </summary>
	private IEqualityEvaluator<TMatch> Evaluator => _evaluator ??= _options.WithoutEvaluation();

	/// <inheritdoc cref="IEqualityEvaluator{TElement}.AreConsideredEqual{TExpected}(TElement, TExpected)" />
	public ValueTask<bool> AreConsideredEqual<TExpected>(TMatch actual, TExpected expected)
		=> Comparer is null
			? Evaluator.AreConsideredEqual(actual, expected)
			: new ValueTask<bool>(Comparer.AreEqual(actual, expected));

	/// <inheritdoc cref="IEqualityEvaluator{TElement}.AppendContexts(ResultContextCollector)" />
	public void AppendContexts(ResultContextCollector contexts)
		=> Evaluator.AppendContexts(contexts);

	/// <summary>
	///     The current options of the wrapped holder.
	/// </summary>
	/// <remarks>
	///     Lets a failure message describe the expected items by the options that decide whenever the comparer of the
	///     subject does not.
	/// </remarks>
	public IEqualityOptions<TMatch> Value => _options.Value;

	/// <inheritdoc cref="ISubjectComparing.UseComparerOf(object?)" />
	public bool UseComparerOf(object? subject)
	{
		Comparer = _canUseSubjectComparer && DefaultEquality.IsUsedBy(Value)
			? CollectionComparerHelpers.GetSubjectComparer<TItem>(subject)
			: null;
		return Comparer is not null;
	}

	/// <inheritdoc cref="IWrapsEqualityEvaluator{T}.Wrap(IEqualityEvaluator{T})" />
	/// <remarks>
	///     The comparer of the subject never decides once the comparison was changed, so the
	///     <paramref name="evaluator" /> is returned as it is then.
	/// </remarks>
	public IEqualityEvaluator<TMatch> Wrap(IEqualityEvaluator<TMatch> evaluator)
	{
		if (!DefaultEquality.IsUsedBy(Value))
		{
			return evaluator;
		}

		_evaluator = evaluator;
		return this;
	}

	/// <inheritdoc cref="object.ToString()" />
	public override string ToString() => $"{Value}{Comparer}";
}
