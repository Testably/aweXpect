using System;
using System.Collections;
using System.Collections.Generic;
using aweXpect.Core;
#if NET8_0_OR_GREATER
using aweXpect.Core.EvaluationContext;
#endif

namespace aweXpect.Helpers;

/// <summary>
///     The "Collection" or "Dictionary" context of a constraint.
/// </summary>
/// <remarks>
///     The constraint only keeps the collection while it is evaluated, and the context is created when the failure
///     message is, so an expectation that is met neither formats nor allocates it.
/// </remarks>
internal struct CollectionContext
{
	private Func<object, bool, int?, string?>? _format;
	private bool _isDictionary;
	private bool _isIncomplete;
	private int? _totalCount;
	private object? _value;

	/// <summary>
	///     Keeps the <paramref name="value" /> for the "Collection" context, together with the <paramref name="totalCount" />
	///     of items whenever the caller counted them while the <paramref name="value" /> kept only the first ones.
	/// </summary>
	public void Set<TItem>(IEnumerable<TItem>? value, bool isIncomplete = false, int? totalCount = null)
		=> Keep(value, TypedFormat<TItem>.Format, isIncomplete, totalCount, false);

	/// <summary>
	///     Keeps the untyped <paramref name="value" /> for the "Collection" context.
	/// </summary>
	public void Set(IEnumerable? value, bool isIncomplete = false)
		=> Keep(value, FormatUntyped, isIncomplete, null, false);

#if NET8_0_OR_GREATER
	/// <summary>
	///     Keeps the <paramref name="value" /> for the "Collection" context of the items that were received when the
	///     failure message is created.
	/// </summary>
	/// <remarks>
	///     No further items are received for the context, so that it can neither delay nor change the outcome. The
	///     context is left out while nothing is known about the items.
	/// </remarks>
	public void Set<TItem>(IMaterializedAsyncEnumerable<TItem>? value, bool isIncomplete = false)
		=> Keep(value, MaterializedFormat<TItem>.Format, isIncomplete, null, false);
#endif

	/// <summary>
	///     Keeps the <paramref name="value" /> for the "Dictionary" context.
	/// </summary>
	public void SetDictionary<TKey, TValue>(IDictionary<TKey, TValue>? value, bool isIncomplete = false)
		=> Keep(value, DictionaryFormat<TKey, TValue>.Format, isIncomplete, null, true);

	/// <summary>
	///     Keeps the <paramref name="value" /> for the "Dictionary" context.
	/// </summary>
	public void SetDictionary<TKey, TValue>(IReadOnlyDictionary<TKey, TValue>? value, bool isIncomplete = false)
		=> Keep(value, ReadOnlyDictionaryFormat<TKey, TValue>.Format, isIncomplete, null, true);

	/// <summary>
	///     Adds the context for the kept collection, if any.
	/// </summary>
	public readonly void AppendTo(ResultContextCollector contexts)
	{
		if (_value is null || _format is null)
		{
			return;
		}

		object value = _value;
		Func<object, bool, int?, string?> format = _format;
		bool isIncomplete = _isIncomplete;
		int? totalCount = _totalCount;
		contexts.Add(_isDictionary
			? new ResultContext.SyncCallback("Dictionary", () => format(value, isIncomplete, totalCount), -2)
			: new ResultContext.SyncCallback("Collection", () => format(value, isIncomplete, totalCount), -1));
	}

	private void Keep(object? value, Func<object, bool, int?, string?> format, bool isIncomplete, int? totalCount,
		bool isDictionary)
	{
		_value = value;
		_format = format;
		_isIncomplete = isIncomplete;
		_totalCount = totalCount;
		_isDictionary = isDictionary;
	}

	private static string? FormatUntyped(object value, bool isIncomplete, int? totalCount)
		=> CollectionHelpers.FormatUntypedCollection((IEnumerable)value)?.AppendIsIncomplete(isIncomplete);

	/// <remarks>
	///     The delegates are cached per type, so that keeping a collection does not allocate.
	/// </remarks>
	private static class TypedFormat<TItem>
	{
		public static readonly Func<object, bool, int?, string?> Format = (value, isIncomplete, totalCount)
			=> CollectionHelpers.FormatCollection((IEnumerable<TItem>)value, totalCount)
				?.AppendIsIncomplete(isIncomplete);
	}

#if NET8_0_OR_GREATER
	private static class MaterializedFormat<TItem>
	{
		public static readonly Func<object, bool, int?, string?> Format = (value, isIncomplete, _) =>
		{
			IMaterializedAsyncEnumerable<TItem> materialized = (IMaterializedAsyncEnumerable<TItem>)value;
			if (materialized.MaterializedItems.Count == 0 && materialized.Count is null)
			{
				return null;
			}

			return materialized.FormatMaterializedItems(FormattingOptions.SingleLine).AppendIsIncomplete(isIncomplete);
		};
	}
#endif

	private static class DictionaryFormat<TKey, TValue>
	{
		public static readonly Func<object, bool, int?, string?> Format = (value, isIncomplete, _) =>
		{
			IDictionary<TKey, TValue> dictionary = (IDictionary<TKey, TValue>)value;
			return Formatter.Format(dictionary, typeof(TValue).GetFormattingOption(dictionary.Count))
				.AppendIsIncomplete(isIncomplete);
		};
	}

	private static class ReadOnlyDictionaryFormat<TKey, TValue>
	{
		public static readonly Func<object, bool, int?, string?> Format = (value, isIncomplete, _) =>
		{
			IReadOnlyDictionary<TKey, TValue> dictionary = (IReadOnlyDictionary<TKey, TValue>)value;
			return Formatter.Format(dictionary, typeof(TValue).GetFormattingOption(dictionary.Count))
				.AppendIsIncomplete(isIncomplete);
		};
	}
}
