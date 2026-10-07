using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using aweXpect.Core.Helpers;
using aweXpect.Customization;

namespace aweXpect.Formatting;

/// <summary>
///     Extension formatting options.
/// </summary>
public static partial class ValueFormatters
{
	/// <summary>
	///     Returns the formatted <paramref name="value" /> according to the <paramref name="options" />.
	/// </summary>
	public static string Format<T>(
		this ValueFormatter formatter,
		IEnumerable<T>? value,
		FormattingOptions? options = null)
	{
		StringBuilder stringBuilder = new();
		formatter.Format(stringBuilder, value, options);
		return stringBuilder.ToString();
	}

	/// <summary>
	///     Returns the formatted <paramref name="value" /> according to the <paramref name="options" />.
	/// </summary>
	public static string Format<TKey, TValue>(
		this ValueFormatter formatter,
		IEnumerable<KeyValuePair<TKey, TValue>>? value,
		FormattingOptions? options = null)
	{
		StringBuilder stringBuilder = new();
		formatter.Format(stringBuilder, value, options);
		return stringBuilder.ToString();
	}

	/// <summary>
	///     Appends the formatted <paramref name="value" /> according to the <paramref name="options" />
	///     to the <paramref name="stringBuilder" />.
	/// </summary>
	public static void Format(
		this ValueFormatter formatter,
		StringBuilder stringBuilder,
		IEnumerable? value,
		FormattingOptions? options = null)
		=> FormatEnumerable(formatter, stringBuilder, value, options, null);

	/// <summary>
	///     Appends the formatted <paramref name="value" /> according to the <paramref name="options" />
	///     to the <paramref name="stringBuilder" />.
	/// </summary>
	public static void Format<T>(
		this ValueFormatter formatter,
		StringBuilder stringBuilder,
		IEnumerable<T>? value,
		FormattingOptions? options = null)
	{
		if (value is null or IDictionary)
		{
			formatter.Format(stringBuilder, (IEnumerable?)value, options);
			return;
		}

		FormattingContext context = _registeredFormatterContext ?? new FormattingContext();
		if (TryFormatWithRegistrations(stringBuilder, value, options, context))
		{
			return;
		}

		FormatItems(stringBuilder, value, value.Cast<object?>(), GetCount(value), options,
			new ItemFormatter<object?>(formatter, context, FormatItem));
	}

	/// <summary>
	///     Appends the formatted <paramref name="value" /> according to the <paramref name="options" />
	///     to the <paramref name="stringBuilder" />.
	/// </summary>
	public static void Format<TKey, TValue>(
		this ValueFormatter formatter,
		StringBuilder stringBuilder,
		IEnumerable<KeyValuePair<TKey, TValue>>? value,
		FormattingOptions? options = null)
	{
		if (value == null)
		{
			stringBuilder.Append(ValueFormatter.NullString);
			return;
		}

		FormattingContext context = _registeredFormatterContext ?? new FormattingContext();
		if (TryFormatWithRegistrations(stringBuilder, value, options, context))
		{
			return;
		}

		FormatItems(stringBuilder, value, value, GetCount(value), options,
			new ItemFormatter<KeyValuePair<TKey, TValue>>(formatter, context, FormatKeyValuePair));
	}

	private static void FormatEnumerable(
		ValueFormatter formatter,
		StringBuilder stringBuilder,
		IEnumerable? value,
		FormattingOptions? options,
		FormattingContext? context)
	{
		if (value == null)
		{
			stringBuilder.Append(ValueFormatter.NullString);
			return;
		}

		context ??= _registeredFormatterContext ?? new FormattingContext();
		if (TryFormatWithRegistrations(stringBuilder, value, options, context))
		{
			return;
		}

		if (value is IDictionary dictionary)
		{
			if (ValueFormatter.Registrations.Length > 0)
			{
				FormatItems(stringBuilder, value, value.Cast<object?>(), GetCount(value), options,
					new ItemFormatter<object?>(formatter, context, FormatDictionaryItem));
			}
			else
			{
				FormatItems(stringBuilder, value, GetEntries(dictionary), GetCount(value), options,
					new ItemFormatter<DictionaryEntry>(formatter, context, FormatDictionaryEntry));
			}
		}
		else if (value is Array { Rank: > 1, } array)
		{
			FormatMultiDimensionalArray(stringBuilder, array, options,
				new ItemFormatter<object?>(formatter, context, FormatItem));
		}
		else
		{
			FormatItems(stringBuilder, value, value.Cast<object?>(), GetCount(value), options,
				new ItemFormatter<object?>(formatter, context, FormatItem));
		}
	}

	/// <remarks>
	///     An array of rank greater than one enumerates its items as one flat sequence, which does not tell its
	///     dimensions, so each dimension is written as a nested collection, like the jagged array with the same items.
	///     The array is tracked as a single collection, like in <see cref="FormatItems{T}" />.
	/// </remarks>
	private static void FormatMultiDimensionalArray(
		StringBuilder stringBuilder,
		Array value,
		FormattingOptions? options,
		ItemFormatter<object?> itemFormatter)
	{
		FormattingContext context = itemFormatter.Context;
		if (!context.FormattedObjects.Add(value))
		{
			stringBuilder.Append("[ *recursive* ]");
			return;
		}

		try
		{
			if (!EnterContent(context))
			{
				stringBuilder.Append("[ \u2026 ]");
				return;
			}

			options ??= FormattingOptions.SingleLine;
			if (options.IncludeType)
			{
				Formatter.Format(stringBuilder, value.GetType());
				stringBuilder.Append(' ');
			}

			new ArrayDimensionWriter(value, options, itemFormatter).Append(stringBuilder, 0);
		}
		finally
		{
			context.Depth--;
			context.FormattedObjects.Remove(value);
		}
	}

	private static string FormatDictionaryEntry(
		ValueFormatter formatter,
		DictionaryEntry item,
		FormattingOptions options,
		FormattingContext context)
	{
		StringBuilder stringBuilder = new();
		AppendKeyValuePair(formatter, stringBuilder, item.Key, item.Value, options with
		{
			IncludeType = false,
			TotalItemCount = null,
		}, context);
		return stringBuilder.ToString();
	}

	/// <remarks>
	///     The entries of a generic dictionary are enumerated as the boxed <see cref="KeyValuePair{TKey,TValue}" />
	///     instead of as <see cref="DictionaryEntry" />, so that the registered formatters are offered the same value as
	///     for a typed dictionary. Without registrations, the entries are read through <see cref="IDictionary" />,
	///     which needs no reflection to read the key and value of a boxed pair.
	/// </remarks>
	private static string FormatDictionaryItem(
		ValueFormatter formatter,
		object? item,
		FormattingOptions options,
		FormattingContext context)
		=> item is DictionaryEntry entry
			? FormatDictionaryEntry(formatter, entry, options, context)
			: FormatItem(formatter, item, options, context);

	private static string FormatItem(
		ValueFormatter formatter,
		object? item,
		FormattingOptions options,
		FormattingContext context)
		=> formatter.Format(item, options with
		{
			IncludeType = false,
			UseLineBreaks = options.UseLineBreaks && item?.GetType() != typeof(string),
			TotalItemCount = null,
		}, context);

	/// <remarks>
	///     The collection is only tracked while its own items are written, so that it is a recursion only within
	///     itself, not when it appears a second time beside itself.
	/// </remarks>
	private static void FormatItems<T>(
		StringBuilder stringBuilder,
		IEnumerable value,
		IEnumerable<T> items,
		int? totalCount,
		FormattingOptions? options,
		ItemFormatter<T> itemFormatter)
	{
		FormattingContext context = itemFormatter.Context;
		bool isDictionary = IsDictionary(value);
		if (!context.FormattedObjects.Add(value))
		{
			stringBuilder.Append(isDictionary ? "{ *recursive* }" : "[ *recursive* ]");
			return;
		}

		int length = stringBuilder.Length;
		try
		{
			if (!EnterContent(context))
			{
				stringBuilder.Append(isDictionary ? "{ \u2026 }" : "[ \u2026 ]");
			}
			else
			{
				AppendItems(stringBuilder, value, isDictionary, items, totalCount, options, itemFormatter);
			}
		}
		catch (Exception exception)
		{
			stringBuilder.Length = length;
			stringBuilder.Append(FormatThrownException("the enumeration", exception));
		}
		finally
		{
			context.Depth--;
			context.FormattedObjects.Remove(value);
		}
	}

#pragma warning disable S3776 // Cognitive Complexity of methods should not be too high
	private static void AppendItems<T>(
		StringBuilder stringBuilder,
		IEnumerable value,
		bool isDictionary,
		IEnumerable<T> items,
		int? totalCount,
		FormattingOptions? options,
		ItemFormatter<T> itemFormatter)
	{
		options ??= FormattingOptions.SingleLine;
		totalCount = options.TotalItemCount ?? totalCount;
		if (options.IncludeType)
		{
			Formatter.Format(stringBuilder, value.GetType());
			stringBuilder.Append(' ');
		}

		int maxCount = Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Get();
		int count = maxCount;
		string itemIndentation = options.Indentation + "  ";

		stringBuilder.Append(isDictionary ? '{' : '[');
		bool hasMoreValues = false;
		bool isNotEmpty = false;
		Exception? enumerationException = null;
		using IEnumerator<T> enumerator = items.GetEnumerator();
		while (TryMoveNext(enumerator, isNotEmpty, ref enumerationException))
		{
			T item = enumerator.Current;
			isNotEmpty = true;
			if (count < maxCount)
			{
				if (options.UseLineBreaks)
				{
					stringBuilder.AppendLine(",");
					stringBuilder.Append(itemIndentation);
				}
				else
				{
					stringBuilder.Append(", ");
				}
			}
			else if (options.UseLineBreaks)
			{
				stringBuilder.AppendLine();
				stringBuilder.Append(itemIndentation);
			}

			if (count-- <= 0)
			{
				hasMoreValues = true;
				break;
			}

			stringBuilder.Append(itemFormatter.FormatSingle(item, options).Indent("  ", false));
		}

		if (enumerationException is not null)
		{
			stringBuilder.Append(options.UseLineBreaks ? $",{Environment.NewLine}{itemIndentation}" : ", ");
			stringBuilder.Append('(').Append(DescribeThrownException("the enumeration", enumerationException))
				.Append(')');
		}

		if (hasMoreValues)
		{
			const char ellipsis = '\u2026';
			stringBuilder.Append('(').Append(ellipsis).Append(" and ");
			if (totalCount > maxCount)
			{
				stringBuilder.Append(totalCount - maxCount);
			}
			else
			{
				stringBuilder.Append("maybe");
			}

			stringBuilder.Append(" more)");
		}

		if (options.UseLineBreaks && isNotEmpty)
		{
			stringBuilder.AppendLine().Append(options.Indentation);
		}

		stringBuilder.Append(isDictionary ? '}' : ']');
	}
#pragma warning restore S3776

	/// <summary>
	///     An exception of the enumeration after the first item is caught, so that the items listed so far are kept.
	/// </summary>
	private static bool TryMoveNext<T>(IEnumerator<T> enumerator, bool isNotEmpty, ref Exception? exception)
	{
		try
		{
			return enumerator.MoveNext();
		}
		catch (Exception caughtException) when (isNotEmpty)
		{
			exception = caughtException;
			return false;
		}
	}

	private static string FormatKeyValuePair<TKey, TValue>(
		ValueFormatter formatter,
		KeyValuePair<TKey, TValue> item,
		FormattingOptions options,
		FormattingContext context)
	{
		FormattingOptions itemOptions = options with
		{
			IncludeType = false,
			TotalItemCount = null,
		};
		StringBuilder stringBuilder = new();
		if (!TryFormatWithRegistrations(stringBuilder, item, itemOptions, context))
		{
			AppendKeyValuePair(formatter, stringBuilder, item.Key, item.Value, itemOptions, context);
		}

		return stringBuilder.ToString();
	}

	/// <summary>
	///     Only reads a count the collection already knows, so that a lazy sequence is not enumerated twice.
	/// </summary>
	/// <remarks>
	///     The count only names the number of remaining items, so a collection that throws when its count is read is
	///     formatted like one that does not know its count.
	/// </remarks>
	private static int? GetCount<T>(IEnumerable<T> value)
	{
		try
		{
			return value switch
			{
				ICollection collection => collection.Count,
				ICollection<T> collection => collection.Count,
				IReadOnlyCollection<T> collection => collection.Count,
				_ => null,
			};
		}
		catch (Exception)
		{
			return null;
		}
	}

	/// <inheritdoc cref="GetCount{T}(IEnumerable{T})" />
	private static int? GetCount(IEnumerable value)
	{
		try
		{
			return (value as ICollection)?.Count;
		}
		catch (Exception)
		{
			return null;
		}
	}

	/// <remarks>
	///     A type that only implements the generic dictionary interfaces is a dictionary as well, so it is rendered in
	///     braces like one.
	/// </remarks>
	private static bool IsDictionary(IEnumerable value)
		=> value is IDictionary ||
		   value.GetType().FindGenericInterface(definition => definition == typeof(IDictionary<,>) ||
		                                                      definition == typeof(IReadOnlyDictionary<,>)) is not null;

	private static IEnumerable<DictionaryEntry> GetEntries(IDictionary dictionary)
	{
		IDictionaryEnumerator enumerator = dictionary.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				yield return enumerator.Entry;
			}
		}
		finally
		{
			(enumerator as IDisposable)?.Dispose();
		}
	}

	/// <summary>
	///     Writes the dimensions of an array of rank greater than one as nested collections.
	/// </summary>
	/// <remarks>
	///     The maximum number of collection items applies to the whole array, so that the items which are not
	///     written are named once, where the next one would follow. A dimension without items counts as one item, so
	///     that an array without any item is limited as well.
	/// </remarks>
	private sealed class ArrayDimensionWriter(
		Array array,
		FormattingOptions options,
		ItemFormatter<object?> itemFormatter)
	{
		private readonly string _itemIndentation = options.Indentation + "  ";
		private readonly IEnumerator _items = array.GetEnumerator();
		private readonly int _maximumCount = Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Get();
		private int _count;
		private bool _isLimited;

		public void Append(StringBuilder stringBuilder, int dimension)
		{
			int length = array.GetLength(dimension);
			if (length == 0)
			{
				_count++;
			}

			stringBuilder.Append('[');
			for (int index = 0; index < length && !_isLimited; index++)
			{
				AppendSeparator(stringBuilder, index == 0);
				if (_count >= _maximumCount)
				{
					_isLimited = true;
					stringBuilder.Append("(\u2026 and ").Append(GetTotalCount() - _maximumCount).Append(" more)");
				}
				else if (dimension == array.Rank - 1)
				{
					_count++;
					_items.MoveNext();
					stringBuilder.Append(itemFormatter.FormatSingle(_items.Current, options).Indent("  ", false));
				}
				else
				{
					StringBuilder nestedBuilder = new();
					Append(nestedBuilder, dimension + 1);
					stringBuilder.Append(nestedBuilder.ToString().Indent("  ", false));
				}
			}

			if (options.UseLineBreaks && length > 0)
			{
				stringBuilder.AppendLine().Append(options.Indentation);
			}

			stringBuilder.Append(']');
		}

		private void AppendSeparator(StringBuilder stringBuilder, bool isFirst)
		{
			if (!isFirst)
			{
				stringBuilder.Append(options.UseLineBreaks ? "," : ", ");
			}

			if (options.UseLineBreaks)
			{
				stringBuilder.AppendLine().Append(_itemIndentation);
			}
		}

		/// <summary>
		///     The number of items, or of dimensions without items when the array has none.
		/// </summary>
		private long GetTotalCount()
		{
			long totalCount = 1;
			for (int dimension = 0; dimension < array.Rank && array.GetLength(dimension) > 0; dimension++)
			{
				totalCount *= array.GetLength(dimension);
			}

			return totalCount;
		}
	}

	/// <summary>
	///     Formats a single item within the formatting context of its collection.
	/// </summary>
	private readonly struct ItemFormatter<T>(
		ValueFormatter formatter,
		FormattingContext context,
		Func<ValueFormatter, T, FormattingOptions, FormattingContext, string> formatItem)
	{
		public FormattingContext Context { get; } = context;

		public string FormatSingle(T item, FormattingOptions options)
			=> formatItem(formatter, item, options, Context);
	}
}
