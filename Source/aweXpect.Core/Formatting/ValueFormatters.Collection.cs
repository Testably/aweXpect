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
		Format(formatter, stringBuilder, value, options);
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
		Format(formatter, stringBuilder, value, options);
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
			Format(formatter, stringBuilder, (IEnumerable?)value, options);
			return;
		}

		if (TryFormatWithRegistrations(stringBuilder, value, options))
		{
			return;
		}

		FormatItems(stringBuilder, value, value.Cast<object?>(), GetCount(value), options,
			new ItemFormatter<object?>(formatter, new FormattingContext(), FormatItem));
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

		if (TryFormatWithRegistrations(stringBuilder, value, options))
		{
			return;
		}

		FormatItems(stringBuilder, value, value, GetCount(value), options,
			new ItemFormatter<KeyValuePair<TKey, TValue>>(formatter, new FormattingContext(), FormatKeyValuePair));
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

		if (TryFormatWithRegistrations(stringBuilder, value, options))
		{
			return;
		}

		context ??= new FormattingContext();
		if (value is IDictionary dictionary)
		{
			FormatItems(stringBuilder, value, GetEntries(dictionary), dictionary.Count, options,
				new ItemFormatter<DictionaryEntry>(formatter, context, FormatDictionaryEntry));
		}
		else
		{
			FormatItems(stringBuilder, value, value.Cast<object?>(), (value as ICollection)?.Count, options,
				new ItemFormatter<object?>(formatter, context, FormatItem));
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

	private static string FormatItem(
		ValueFormatter formatter,
		object? item,
		FormattingOptions options,
		FormattingContext context)
		=> Format(formatter, item, options with
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
		if (!context.FormattedObjects.Add(value))
		{
			stringBuilder.Append(value is IDictionary ? "{*recursive*}" : "[*recursive*]");
			return;
		}

		int length = stringBuilder.Length;
		try
		{
			AppendItems(stringBuilder, value, items, totalCount, options, itemFormatter);
		}
		catch (Exception exception)
		{
			stringBuilder.Length = length;
			stringBuilder.Append(FormatThrownException("the enumeration", exception));
		}
		finally
		{
			context.FormattedObjects.Remove(value);
		}
	}

#pragma warning disable S3776 // Cognitive Complexity of methods should not be too high
	private static void AppendItems<T>(
		StringBuilder stringBuilder,
		IEnumerable value,
		IEnumerable<T> items,
		int? totalCount,
		FormattingOptions? options,
		ItemFormatter<T> itemFormatter)
	{
		options ??= FormattingOptions.SingleLine;
		totalCount = options.TotalItemCount ?? totalCount;
		if (options.IncludeType)
		{
			Format(Formatter, stringBuilder, value.GetType());
			stringBuilder.Append(' ');
		}

		int maxCount = Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Get();
		int count = maxCount;

		bool isDictionary = value is IDictionary;
		stringBuilder.Append(isDictionary ? '{' : '[');
		bool hasMoreValues = false;
		bool isNotEmpty = false;
		foreach (T item in items)
		{
			isNotEmpty = true;
			if (count < maxCount)
			{
				if (options.UseLineBreaks)
				{
					stringBuilder.AppendLine(",");
					stringBuilder.Append("  ");
				}
				else
				{
					stringBuilder.Append(", ");
				}
			}
			else if (options.UseLineBreaks)
			{
				stringBuilder.AppendLine();
				stringBuilder.Append("  ");
			}

			if (count-- <= 0)
			{
				hasMoreValues = true;
				break;
			}

			stringBuilder.Append(itemFormatter.Format(item, options).Indent("  ", false));
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
			stringBuilder.AppendLine();
		}

		stringBuilder.Append(isDictionary ? '}' : ']');
	}
#pragma warning restore S3776

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
		if (!TryFormatWithRegistrations(stringBuilder, item, itemOptions))
		{
			AppendKeyValuePair(formatter, stringBuilder, item.Key, item.Value, itemOptions, context);
		}

		return stringBuilder.ToString();
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

		public string Format(T item, FormattingOptions options)
			=> formatItem(formatter, item, options, Context);
	}

	/// <summary>
	///     Only reads a count the collection already knows, so that a lazy sequence is not enumerated twice.
	/// </summary>
	private static int? GetCount<T>(IEnumerable<T> value)
		=> value switch
		{
			ICollection collection => collection.Count,
			ICollection<T> collection => collection.Count,
			IReadOnlyCollection<T> collection => collection.Count,
			_ => null,
		};

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
}
