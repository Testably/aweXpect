using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Customization;
using aweXpect.Equivalency;
using aweXpect.Options;

namespace aweXpect.Core;

/// <summary>
///     Adds the standard contexts of the built-in expectations to a <see cref="ResultContextCollector" />.
/// </summary>
/// <remarks>
///     Call them from <see cref="ConstraintResult.AppendContexts(ResultContextCollector)" />, which runs only while the
///     failure message is created, so an expectation that is met neither formats nor allocates the contexts.
/// </remarks>
public static class ResultContextCollectorExtensions
{
	/// <summary>
	///     Adds the "Collection" context, which lists the items of the <paramref name="collection" />.
	/// </summary>
	/// <param name="contexts">The collector to add the context to.</param>
	/// <param name="collection">
	///     The collection to list, or <see langword="null" /> to add no context. Only the first items are listed, so an
	///     endless collection is not enumerated to its end.
	/// </param>
	/// <param name="isIncomplete">
	///     Whether the enumeration stopped before the end of the collection, e.g. after a cancellation, which marks the
	///     list with <c>(… and maybe more)</c>.
	/// </param>
	/// <param name="totalCount">
	///     The number of items in total, when the <paramref name="collection" /> holds only the first of them, e.g. the
	///     ones that were kept while the items were counted. The layout then follows the held items, and the list ends
	///     with the number of items that are not listed.
	/// </param>
	/// <remarks>
	///     The items read so far of a collection that is still being read, e.g. an <see cref="IMaterializedEnumerable{T}" />
	///     whose end is not reached yet, are listed as incomplete without reading further items, and no context is
	///     added while no item was read.
	/// </remarks>
	public static void AddCollectionContext<TItem>(this ResultContextCollector contexts,
		IEnumerable<TItem>? collection, bool isIncomplete = false, int? totalCount = null)
	{
		if (collection is not null)
		{
			contexts.Add(new ResultContext.SyncCallback("Collection",
				() => FormatCollection(collection, totalCount)?.AppendIsIncomplete(isIncomplete), -1));
		}
	}

	/// <inheritdoc cref="AddCollectionContext{TItem}(ResultContextCollector, IEnumerable{TItem}, bool, int?)" />
	/// <remarks>
	///     The layout follows the type of the first listed item that is not <see langword="null" />.
	/// </remarks>
	public static void AddCollectionContext(this ResultContextCollector contexts,
		IEnumerable? collection, bool isIncomplete = false)
	{
		if (collection is not null)
		{
			contexts.Add(new ResultContext.SyncCallback("Collection",
				() => FormatUntypedCollection(collection)?.AppendIsIncomplete(isIncomplete), -1));
		}
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Adds the "Collection" context, which lists the items of the <paramref name="collection" /> that were received
	///     so far.
	/// </summary>
	/// <param name="contexts">The collector to add the context to.</param>
	/// <param name="collection">The collection to list, or <see langword="null" /> to add no context.</param>
	/// <param name="isIncomplete">
	///     Whether the enumeration stopped before the end of the collection, e.g. after a cancellation, which marks the
	///     list with <c>(… and maybe more)</c>.
	/// </param>
	/// <remarks>
	///     No further items are received for the context, so that it can neither delay nor change the outcome. The
	///     context is left out while nothing is known about the items.
	/// </remarks>
	public static void AddCollectionContext<TItem>(this ResultContextCollector contexts,
		IMaterializedAsyncEnumerable<TItem>? collection, bool isIncomplete = false)
	{
		if (collection is not null)
		{
			contexts.Add(new ResultContext.SyncCallback("Collection",
				() => FormatMaterializedItems(collection)?.AppendIsIncomplete(isIncomplete), -1));
		}
	}
#endif

	/// <summary>
	///     Adds the "Dictionary" context, which lists the entries of the <paramref name="dictionary" />, when it is an
	///     <see cref="IDictionary{TKey, TValue}" /> or an <see cref="IReadOnlyDictionary{TKey, TValue}" />.
	/// </summary>
	/// <param name="contexts">The collector to add the context to.</param>
	/// <param name="dictionary">The dictionary to list, or <see langword="null" /> to add no context.</param>
	public static void AddDictionaryContext<TKey, TValue>(this ResultContextCollector contexts,
		IEnumerable<KeyValuePair<TKey, TValue>>? dictionary)
	{
		if (dictionary is IDictionary<TKey, TValue> mutableDictionary)
		{
			contexts.Add(new ResultContext.SyncCallback("Dictionary",
				() => Formatter.Format(mutableDictionary,
					typeof(TValue).GetFormattingOption(ValueFormatters.GetCount(mutableDictionary))), -2));
		}
		else if (dictionary is IReadOnlyDictionary<TKey, TValue> readOnlyDictionary)
		{
			contexts.Add(new ResultContext.SyncCallback("Dictionary",
				() => Formatter.Format(readOnlyDictionary,
					typeof(TValue).GetFormattingOption(ValueFormatters.GetCount(readOnlyDictionary))), -2));
		}
	}

	/// <summary>
	///     Adds the contexts of the match type of the equality <paramref name="options" />, e.g. the
	///     "Equivalency options".
	/// </summary>
	/// <param name="contexts">The collector to add the contexts to.</param>
	/// <param name="options">
	///     The equality options, which are an <see cref="ObjectEqualityOptions{TSubject}" /> or provide one.
	/// </param>
	public static void AddEqualityOptionsContexts<T>(this ResultContextCollector contexts,
		IOptionsEquality<T> options)
	{
		if (options is IOptionsProvider<IOptionsEquality<T>> provider)
		{
			contexts.AddEqualityOptionsContexts(provider.Options);
		}
		else if (options is ObjectEqualityOptions<T> objectEqualityOptions)
		{
			objectEqualityOptions.AppendContexts(contexts);
		}
	}

	/// <summary>
	///     Adds the "Equivalency options" context, which lists the <paramref name="equivalencyOptions" />.
	/// </summary>
	/// <param name="contexts">The collector to add the context to.</param>
	/// <param name="equivalencyOptions">The options of the equivalency comparison.</param>
	public static void AddEquivalencyContext(this ResultContextCollector contexts,
		EquivalencyOptions equivalencyOptions)
		=> contexts.Add(new ResultContext.SyncCallback("Equivalency options", equivalencyOptions.ToString,
			int.MinValue));

	/// <summary>
	///     Adds the "Expected values" context, or the "Unexpected values" context for negated
	///     <paramref name="grammars" />, which lists the <paramref name="values" />, when the failure message names only
	///     the <paramref name="expectedExpression" /> instead of the values themselves.
	/// </summary>
	/// <param name="contexts">The collector to add the context to.</param>
	/// <param name="expectedExpression">
	///     The expression of the expected values in the failure message, or <see langword="null" /> when it lists the
	///     values themselves, which adds no context.
	/// </param>
	/// <param name="values">
	///     The expected values. Only the first values are listed, so an endless sequence is not enumerated to its end.
	/// </param>
	/// <param name="grammars">The grammars of the result, whose final negation decides the title.</param>
	public static void AddExpectedValuesContext<TItem>(this ResultContextCollector contexts,
		string? expectedExpression, IEnumerable<TItem> values, ExpectationGrammars grammars)
	{
		if (expectedExpression is not null)
		{
			contexts.Add(new ResultContext.SyncCallback(grammars.IsNegated() ? "Unexpected values" : "Expected values",
				() => Formatter.Format(values)));
		}
	}

	/// <summary>
	///     Adds the full <paramref name="value" /> as a context with the <paramref name="title" />, unless it is empty or
	///     the failure message of the <paramref name="result" /> already shows it completely.
	/// </summary>
	/// <param name="contexts">The collector to add the context to.</param>
	/// <param name="title">The title of the context, e.g. <c>Expected</c>.</param>
	/// <param name="value">The string to show.</param>
	/// <param name="result">The result whose failure message might shorten the <paramref name="value" />.</param>
	/// <remarks>
	///     The check reads the text of the <paramref name="result" />, so it runs when the context is added, before the
	///     result can be evaluated again for another item of a collection.
	/// </remarks>
	public static void AddStringContext(this ResultContextCollector contexts, string title, string? value,
		ConstraintResult result)
	{
		if (!string.IsNullOrEmpty(value) && !IsShownCompletely(value!, result))
		{
			contexts.Add(new ResultContext.Fixed(title, value));
		}
	}

	private static bool IsShownCompletely(string value, ConstraintResult result)
	{
		// The formatter escapes line breaks and tabs before it shortens the value, which still shows it completely.
		if (GetEscapedLength(value) > Customize.aweXpect.Formatting().MaximumStringLength.Get())
		{
			return false;
		}

		// Some match types shorten the value further, so it must actually appear in the rendered text.
		string formatted = Formatter.Format(value);
		StringBuilder stringBuilder = new();
		result.AppendExpectation(stringBuilder);
		result.AppendResult(stringBuilder);
		return stringBuilder.ToString().Contains(formatted);
	}

	private static int GetEscapedLength(string value)
		=> value.Length + value.Count(c => c is '\n' or '\r' or '\t');

	/// <remarks>
	///     When the <paramref name="totalCount" /> is known, the listed items are only the first ones, so the layout
	///     follows their number, while the total names the remainder.
	/// </remarks>
	private static string? FormatCollection<TItem>(IEnumerable<TItem> value, int? totalCount)
	{
		if (value is IKeyedCollection keyed)
		{
			return keyed.Format();
		}

		if (totalCount is not null)
		{
			int? count = ValueFormatters.GetCount(value) ?? CountItems(value);
			return Formatter.Format(value, typeof(TItem).GetFormattingOption(count, totalCount));
		}

		if (value is IMaterializedEnumerable<TItem> { Count: null, } materialized)
		{
			return FormatReadItems(materialized.MaterializedItems, typeof(TItem));
		}

		totalCount = ValueFormatters.GetCount(value) ?? (value as ICountable)?.Count;
		return Formatter.Format(value, typeof(TItem).GetFormattingOption(totalCount, totalCount));
	}

	/// <remarks>
	///     The count only decides the layout, so items whose enumeration throws are laid out like items of an unknown
	///     number, and the formatter renders the exception.
	/// </remarks>
	private static int? CountItems<TItem>(IEnumerable<TItem> value)
	{
		try
		{
			return value.Count();
		}
		catch (Exception)
		{
			return null;
		}
	}

	private static string? FormatUntypedCollection(IEnumerable value)
	{
		if (value is IMaterializedEnumerable { Count: null, } materialized)
		{
			return FormatReadItems(materialized.MaterializedItems, GetItemType(materialized.MaterializedItems));
		}

		int? totalCount = ValueFormatters.GetCount(value) ?? (value as ICountable)?.Count;
		return Formatter.Format(value,
			GetItemTypeOfListedItems(value, totalCount is not null).GetFormattingOption(totalCount, totalCount));
	}

#if NET8_0_OR_GREATER
	private static string? FormatMaterializedItems<TItem>(IMaterializedAsyncEnumerable<TItem> value)
	{
		if (value.MaterializedItems.Count == 0 && value.Count is null)
		{
			return null;
		}

		int count = value.Count ?? value.MaterializedItems.Count;
		return Formatter.Format(HideCount(value.MaterializedItems), typeof(TItem).GetFormattingOption(count, value.Count))
			.AppendIsIncomplete(value.Count is null);
	}
#endif

	/// <remarks>
	///     No further items are read for the context, so that a source that blocks cannot hang the failure message. The
	///     context is left out while nothing is known about the items.
	/// </remarks>
	private static string? FormatReadItems<TItem>(IReadOnlyList<TItem> items, Type itemType)
		=> items.Count == 0
			? null
			: Formatter.Format(HideCount(items), itemType.GetFormattingOption(items.Count)).AppendIsIncomplete(true);

	/// <summary>
	///     The materialized items can be only the first items of the source, so their count must not be rendered as the
	///     number of remaining items.
	/// </summary>
	private static IEnumerable<TItem> HideCount<TItem>(IEnumerable<TItem> items)
	{
		foreach (TItem item in items)
		{
			yield return item;
		}
	}

	/// <remarks>
	///     Only the first items are listed, so an endless source of <see langword="null" /> items must not be searched
	///     to its end, unless it <paramref name="knowsItsCount" />. An exception of the source is ignored here, as the
	///     formatter enumerates the same items and renders it.
	/// </remarks>
	private static Type GetItemTypeOfListedItems(IEnumerable value, bool knowsItsCount)
	{
		IEnumerable<object?> items = knowsItsCount
			? value.Cast<object?>()
			: value.Cast<object?>().Take(Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Get());
		try
		{
			return GetItemType(items);
		}
		catch (Exception)
		{
			return typeof(object);
		}
	}

	private static Type GetItemType(IEnumerable<object?> items)
		=> items.FirstOrDefault(item => item is not null)?.GetType() ?? typeof(object);
}
