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
	private bool _isEmpty;
	private bool _isNull;

	/// <summary>
	///     The number of matching items so far.
	/// </summary>
	protected int Count { get; private set; }

	/// <summary>
	///     The single matching item, when the expectation is met.
	/// </summary>
	/// <remarks>
	///     Further expectations on the item read it here instead of searching the collection again, which would call the
	///     predicate a second time.
	/// </remarks>
	internal TItem? SingleItem => Actual;

	/// <summary>
	///     The materialized collection, which is served instead of the subject.
	/// </summary>
	protected abstract object? Materialized { get; }

	/// <summary>
	///     Starts a new evaluation of a subject which is <see langword="null" />, when <paramref name="isNull" /> is set.
	/// </summary>
	protected void Start(bool isNull)
	{
		_isNull = isNull;
		Count = 0;
		_isEmpty = true;
		if (isNull)
		{
			Outcome = Outcome.FailureBothWays;
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
		return ++Count > 1;
	}

	/// <summary>
	///     Determines the outcome from the recorded items.
	/// </summary>
	protected void Complete()
		=> Outcome = Count == 1 ? Outcome.Success : Outcome.Failure;

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
		else if (Count == 0)
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
			stringBuilder.Append(Formatter.Format(Actual).Indent(indentation, false));
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

	public ValueTask<ConstraintResult> IsMetBy(TEnumerable actual, IEvaluationContext context,
		CancellationToken cancellationToken)
	{
		_collectionContext = default;
		_materialized = null;
		Start(actual is null);
		if (actual.IsDefaultImmutableArray())
		{
			return new ValueTask<ConstraintResult>(this.AsNullSubject(It));
		}

		if (actual is null)
		{
			return new ValueTask<ConstraintResult>(this);
		}

		CollectionItems<TItem> materialized = CollectionItems<TItem>.Materialize(actual, context);
		_materialized = materialized.Value;
		foreach (TItem item in materialized)
		{
			if (materialized.IsCanceledBeforeTheEnd(cancellationToken))
			{
				Outcome = Outcome.Undecided;
				materialized.SetContext(ref _collectionContext, true);
				return new ValueTask<ConstraintResult>(this);
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

		return new ValueTask<ConstraintResult>(this);
	}

	/// <inheritdoc />
	public override void AppendContexts(ResultContextCollector contexts)
		=> _collectionContext.AppendTo(contexts);
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

	public async ValueTask<ConstraintResult> IsMetBy(IAsyncEnumerable<TItem>? actual, IEvaluationContext context,
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
		await using (IAsyncEnumerator<TItem> items =
		             materialized.UntilCancelled(cancellationToken).GetAsyncEnumerator(CancellationToken.None))
		{
			bool isDecided = false;
			while (!isDecided && await items.MoveNextAsync())
			{
				isDecided = Record(items.Current);
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

	/// <inheritdoc />
	public override void AppendContexts(ResultContextCollector contexts)
		=> _collectionContext.AppendTo(contexts);
}
#endif
