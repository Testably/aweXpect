using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Helpers;
using aweXpect.Options;
#if NET8_0_OR_GREATER
using System.Collections.Generic;
#endif

namespace aweXpect;

/// <summary>
///     The evaluation of the items and the texts of <c>HasSingle</c>, shared by the synchronous and the asynchronous
///     collections.
/// </summary>
internal abstract class HasSingleConstraintBase<TValue, TItem>(
	string it,
	ExpectationGrammars grammars,
	PredicateOptions<TItem> options)
	: ConstraintResult.WithValue<TItem?>(it, grammars)
{
	private int _count;
	private bool _isEmpty;
	private bool _isNull;

	/// <summary>
	///     The number of matching items so far.
	/// </summary>
	protected int Count => _count;

	/// <summary>
	///     The materialized collection, which is served instead of the subject.
	/// </summary>
	protected abstract object? Materialized { get; }

	/// <inheritdoc cref="ConstraintResult.Outcome" />
	public override Outcome Outcome
	{
		get => _isNull ? Outcome.Failure : base.Outcome;
		protected set => base.Outcome = value;
	}

	/// <summary>
	///     Starts a new evaluation of a subject which is <see langword="null" />, when <paramref name="isNull" /> is set.
	/// </summary>
	protected void Start(bool isNull)
	{
		_isNull = isNull;
		_count = 0;
		_isEmpty = true;
		if (isNull)
		{
			Outcome = Outcome.Failure;
		}
	}

	/// <summary>
	///     Records the <paramref name="item" /> and returns <see langword="true" />, when the remaining items cannot change
	///     the outcome.
	/// </summary>
	protected bool Record(TItem item)
	{
		_isEmpty = false;
		if (!options.Matches(item))
		{
			return false;
		}

		Actual = item;
		return ++_count > 1;
	}

	/// <summary>
	///     Determines the outcome from the recorded items.
	/// </summary>
	protected void Complete()
		=> Outcome = _count == 1 ? Outcome.Success : Outcome.Failure;

	/// <remarks>
	///     The collection is served from the materialized items, so that the single item for further expectations
	///     does not enumerate the source again. Only the exact collection type is served, as the collection itself can
	///     also be an item (e.g. an <see langword="object" />).
	/// </remarks>
	public override bool TryGetStoredValue<T>(out T? value) where T : default
	{
		if (typeof(T) == typeof(TValue) &&
		    Materialized is T typedValue)
		{
			value = typedValue;
			return true;
		}

		return base.TryGetStoredValue(out value);
	}

	protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		=> stringBuilder.Append(Grammars.Verb("has a single item", "have a single item"))
			.Append(options.GetDescription());

	protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
	{
		if (_isNull)
		{
			stringBuilder.ItWasNull(It, Grammars);
		}
		else if (_count == 0)
		{
			stringBuilder.Append(It).Append(_isEmpty
				? Grammars.SubjectVerb(It, " was empty", " were empty")
				: " had no matching item");
		}
		else
		{
			stringBuilder.Append(It).Append(options.GetDescription().Length == 0
				? " had more than one item"
				: " had more than one matching item");
		}
	}

	protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		=> stringBuilder.Append(Grammars.Verb("does not have a single item", "do not have a single item"))
			.Append(options.GetDescription());

	protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
	{
		if (_isNull)
		{
			stringBuilder.ItWasNull(It, Grammars);
		}
		else
		{
			stringBuilder.Append(It).Append(options.GetDescription().Length == 0
				? " had the single item "
				: " had the single matching item ");
			Formatter.Format(stringBuilder, Actual);
		}
	}
}

internal sealed class HasSingleConstraint<TEnumerable, TItem>(
	string it,
	ExpectationGrammars grammars,
	PredicateOptions<TItem> options)
	: HasSingleConstraintBase<TEnumerable, TItem>(it, grammars, options),
		IAsyncContextConstraint<TEnumerable>
	where TEnumerable : IEnumerable?
{
	private CollectionContext _collectionContext;
	private object? _materialized;

	/// <inheritdoc />
	protected override object? Materialized => _materialized;

	/// <inheritdoc />
	public override void AppendContexts(ResultContextCollector contexts)
		=> _collectionContext.AppendTo(contexts);

	public Task<ConstraintResult> IsMetBy(TEnumerable actual, IEvaluationContext context,
		CancellationToken cancellationToken)
	{
		_collectionContext = default;
		_materialized = null;
		Start(actual is null);
		if (actual.IsDefaultImmutableArray())
		{
			return Task.FromResult(this.AsNullSubject(It));
		}

		if (actual is null)
		{
			return Task.FromResult<ConstraintResult>(this);
		}

		CollectionItems<TItem> materialized = CollectionItems<TItem>.Materialize(actual, context);
		_materialized = materialized.Value;
		foreach (TItem item in materialized.Items)
		{
			if (materialized.IsCanceledBeforeTheEnd(cancellationToken))
			{
				Outcome = Outcome.Undecided;
				materialized.SetContext(ref _collectionContext, true);
				return Task.FromResult<ConstraintResult>(this);
			}

			if (Record(item))
			{
				break;
			}
		}

		Complete();
		// The single item also explains the failure of a negation, but not of a continuation on the item.
		if (Count > 0)
		{
			materialized.SetContext(ref _collectionContext);
		}

		return Task.FromResult<ConstraintResult>(this);
	}
}

#if NET8_0_OR_GREATER
internal sealed class AsyncHasSingleConstraint<TItem>(
	string it,
	ExpectationGrammars grammars,
	PredicateOptions<TItem> options)
	: HasSingleConstraintBase<IAsyncEnumerable<TItem>, TItem>(it, grammars, options),
		IAsyncContextConstraint<IAsyncEnumerable<TItem>?>
{
	private CollectionContext _collectionContext;
	private IMaterializedAsyncEnumerable<TItem>? _materialized;

	/// <inheritdoc />
	protected override object? Materialized => _materialized;

	/// <inheritdoc />
	public override void AppendContexts(ResultContextCollector contexts)
		=> _collectionContext.AppendTo(contexts);

	public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<TItem>? actual, IEvaluationContext context,
		CancellationToken cancellationToken)
	{
		_collectionContext = default;
		_materialized = null;
		Start(actual is null);
		if (actual is null)
		{
			return this;
		}

		IAsyncEnumerable<TItem> materialized =
			context.UseMaterializedAsyncEnumerable<TItem>(actual, cancellationToken);
		_materialized = materialized as IMaterializedAsyncEnumerable<TItem>;
		await foreach (TItem item in materialized.UntilCancelled(cancellationToken))
		{
			if (Record(item))
			{
				break;
			}
		}

		if (Count <= 1 && cancellationToken.IsCanceledBeforeTheEndOf(materialized))
		{
			Outcome = Outcome.Undecided;
			_collectionContext.Set(_materialized, true);
			return this;
		}

		Complete();
		// The single item also explains the failure of a negation, but not of a continuation on the item.
		if (Count > 1)
		{
			_collectionContext.Set(_materialized);
		}
		else if (Count == 1)
		{
			_collectionContext.Set(_materialized?.MaterializedItems);
		}

		return this;
	}
}
#endif
