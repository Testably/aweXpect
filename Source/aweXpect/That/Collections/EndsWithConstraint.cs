using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Helpers;
using aweXpect.Options;

namespace aweXpect;

/// <summary>
///     The comparison of the last items and the texts of <c>EndsWith</c>, shared by the synchronous and the
///     asynchronous collections.
/// </summary>
internal abstract class EndsWithConstraintBase<TValue, TItem, TMatch>(
	string it,
	ExpectationGrammars grammars,
	string expectedExpression,
	TMatch[] expected,
	IOptionsEquality<TMatch> options)
	: ConstraintResult.WithNotNullValue<TValue>(it, grammars)
{
	private TItem? _firstMismatchItem;
	private bool _foundMismatch;
	private int _index;
	private IList<TItem>? _items;
	private int _itemsCount;
	private int _offset;

	/// <inheritdoc />
	public override void AppendContexts(ResultContextCollector contexts)
		=> contexts.AddOptionsContexts(options);

	/// <summary>
	///     Starts a new evaluation.
	/// </summary>
	protected void Start()
	{
		_firstMismatchItem = default;
		_foundMismatch = false;
		_items = null;
	}

	/// <summary>
	///     Compares the last of the <paramref name="items" /> with the expected items and returns whether they match.
	/// </summary>
	protected async Task<bool> CompareEnd(IList<TItem> items)
	{
		_items = items;
		_itemsCount = items.Count;
		_offset = _itemsCount - expected.Length;
		for (_index = expected.Length - 1; _index >= 0; _index--)
		{
			if (_index + _offset < 0)
			{
				Outcome = Outcome.Failure;
				return false;
			}

			TItem item = items[_index + _offset];
			if (!CollectionItems<TItem>.TryCast(item, out TMatch matchedItem) ||
			    !await options.AreConsideredEqual(matchedItem, expected[_index]))
			{
				_firstMismatchItem = item;
				_foundMismatch = true;
				Outcome = Outcome.Failure;
				return false;
			}
		}

		Outcome = Outcome.Success;
		return true;
	}

	/// <summary>
	///     Appends the last <paramref name="items" />, which matched the expected items.
	/// </summary>
	protected virtual void AppendMatchingItems(StringBuilder stringBuilder, IEnumerable<TItem> items, int count)
		=> Formatter.Format(stringBuilder, items, typeof(TItem).GetFormattingOption(count));

	protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
	{
		stringBuilder.Append(Grammars.Verb("ends with ", "end with ")).Append(expectedExpression);
		stringBuilder.Append(options);
	}

	protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
	{
		if (_foundMismatch)
		{
			stringBuilder.Append(It).Append(" contained item ");
			Formatter.Format(stringBuilder, _firstMismatchItem);
			stringBuilder.Append(" at index ").Append(_index + _offset).Append(" instead of ");
			stringBuilder.AppendExpectedItem(expected[_index], options);
		}
		else
		{
			stringBuilder.Append(It).Append(" contained only ").AppendItemCount(_itemsCount).Append(" and lacked ")
				.AppendItemCount(expected.Length - _itemsCount).Append(": ");
			Formatter.Format(stringBuilder, expected.Take(-_offset), FormattingOptions.MultipleLines);
		}
	}

	protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
	{
		stringBuilder.Append(Grammars.Verb("does not end with ", "do not end with ")).Append(expectedExpression);
		stringBuilder.Append(options);
	}

	protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
	{
		stringBuilder.Append(It).Append(" did end with ");
		AppendMatchingItems(stringBuilder, _items?.Skip(_offset) ?? [], expected.Length);
	}
}

internal sealed class EndsWithConstraint<TEnumerable, TItem, TMatch>(
	string it,
	ExpectationGrammars grammars,
	string expectedExpression,
	TMatch[] expected,
	IOptionsEquality<TMatch> options,
	Func<object?, bool>? useComparerOf = null)
	: EndsWithConstraintBase<TEnumerable, TItem, TMatch>(it, grammars, expectedExpression, expected, options),
		IAsyncContextConstraint<TEnumerable>
	where TEnumerable : IEnumerable?
{
	private CollectionContext _collectionContext;

	/// <inheritdoc />
	public override void AppendContexts(ResultContextCollector contexts)
	{
		_collectionContext.AppendTo(contexts);
		base.AppendContexts(contexts);
	}

	public async ValueTask<ConstraintResult> IsMetBy(TEnumerable actual, IEvaluationContext context,
		CancellationToken cancellationToken)
	{
		_collectionContext = default;
		Start();
		Actual = actual;
		if (actual.IsDefaultImmutableArray())
		{
			return this.AsNullSubject(It);
		}

		if (actual is null)
		{
			Outcome = Outcome.Failure;
			return this;
		}

		useComparerOf?.Invoke(actual);
		CollectionItems<TItem> materialized = CollectionItems<TItem>.Materialize(actual, context);
		IList<TItem> items;
		if (materialized.Value is TItem[] or List<TItem>)
		{
			// The items are already in memory, and reading them by index calls no code of the caller.
			items = (IList<TItem>)materialized.Value;
		}
		else
		{
			List<TItem> readItems = [];
			foreach (TItem item in materialized)
			{
				if (materialized.IsCanceledBeforeTheEnd(cancellationToken))
				{
					Outcome = Outcome.Undecided;
					materialized.SetContext(ref _collectionContext, true);
					return this;
				}

				readItems.Add(item);
			}

			items = readItems;
		}

		if (!await CompareEnd(items))
		{
			materialized.SetContext(ref _collectionContext);
		}

		return this;
	}

	/// <remarks>
	///     The items of a non-generic collection are laid out by the type of the first one that is not
	///     <see langword="null" />.
	/// </remarks>
	protected override void AppendMatchingItems(StringBuilder stringBuilder, IEnumerable<TItem> items, int count)
	{
		Type itemType = CollectionItems<TItem>.IsTyped<TEnumerable>()
			? typeof(TItem)
			: items.Cast<object?>().GetItemType();
		Formatter.Format(stringBuilder, items, itemType.GetFormattingOption(count));
	}
}

#if NET8_0_OR_GREATER
internal sealed class AsyncEndsWithConstraint<TItem, TMatch>(
	string it,
	ExpectationGrammars grammars,
	string expectedExpression,
	TMatch[] expected,
	IOptionsEquality<TMatch> options)
	: EndsWithConstraintBase<IAsyncEnumerable<TItem>?, TItem, TMatch>(it, grammars, expectedExpression, expected,
			options),
		IAsyncContextConstraint<IAsyncEnumerable<TItem>?>
{
	private CollectionContext _collectionContext;

	/// <inheritdoc />
	public override void AppendContexts(ResultContextCollector contexts)
	{
		_collectionContext.AppendTo(contexts);
		base.AppendContexts(contexts);
	}

	public async ValueTask<ConstraintResult> IsMetBy(IAsyncEnumerable<TItem>? actual, IEvaluationContext context,
		CancellationToken cancellationToken)
	{
		_collectionContext = default;
		Start();
		Actual = actual;
		if (actual is null)
		{
			Outcome = Outcome.Failure;
			return this;
		}

		IAsyncEnumerable<TItem> materializedEnumerable =
			context.UseMaterializedAsyncEnumerable<TItem>(actual, cancellationToken);
		List<TItem> items = [];
		await foreach (TItem item in materializedEnumerable.UntilCancelled(cancellationToken))
		{
			items.Add(item);
		}

		if (cancellationToken.IsCanceledBeforeTheEndOf(materializedEnumerable))
		{
			Outcome = Outcome.Undecided;
			_collectionContext.Set(materializedEnumerable as IMaterializedAsyncEnumerable<TItem>, true);
			return this;
		}

		if (!await CompareEnd(items))
		{
			_collectionContext.Set(materializedEnumerable as IMaterializedAsyncEnumerable<TItem>);
		}

		return this;
	}
}
#endif
