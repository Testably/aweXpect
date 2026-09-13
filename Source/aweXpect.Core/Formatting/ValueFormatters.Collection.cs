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
		Format(formatter, stringBuilder, (IEnumerable?)value, options);
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
	///     to the <paramref name="stringBuilder" />
	/// </summary>
	public static void Format(
		this ValueFormatter formatter,
		StringBuilder stringBuilder,
		IEnumerable? value,
		FormattingOptions? options = null)
	{
		if (value == null)
		{
			stringBuilder.Append(ValueFormatter.NullString);
		}
		else if (value is IDictionary dictionary)
		{
			FormatItems(formatter, stringBuilder, value, GetEntries(dictionary), options, FormatDictionaryEntry);
		}
		else
		{
			FormatItems(formatter, stringBuilder, value, value.Cast<object?>(), options, FormatItem);
		}
	}

	/// <summary>
	///     Appends the formatted <paramref name="value" /> according to the <paramref name="options" />
	///     to the <paramref name="stringBuilder" />
	/// </summary>
	public static void Format<T>(
		this ValueFormatter formatter,
		StringBuilder stringBuilder,
		IEnumerable<T>? value,
		FormattingOptions? options = null)
		=> Format(formatter, stringBuilder, (IEnumerable?)value, options);

	/// <summary>
	///     Appends the formatted <paramref name="value" /> according to the <paramref name="options" />
	///     to the <paramref name="stringBuilder" />
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

		FormatItems(formatter, stringBuilder, value, value, options, FormatKeyValuePair);
	}

	private static string FormatDictionaryEntry(
		ValueFormatter formatter,
		DictionaryEntry item,
		FormattingOptions options)
		=> Format(formatter, new KeyValuePair<object?, object?>(item.Key, item.Value), options with
		{
			IncludeType = false,
		});

	private static string FormatItem(
		ValueFormatter formatter,
		object? item,
		FormattingOptions options)
		=> Format(formatter, item, options with
		{
			IncludeType = false,
			UseLineBreaks = options.UseLineBreaks && item?.GetType() != typeof(string),
		});

#pragma warning disable S3776 // Cognitive Complexity of methods should not be too high
	private static void FormatItems<T>(
		ValueFormatter formatter,
		StringBuilder stringBuilder,
		IEnumerable value,
		IEnumerable<T> items,
		FormattingOptions? options,
		Func<ValueFormatter, T, FormattingOptions, string> formatItem)
	{
		options ??= FormattingOptions.SingleLine;
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

			stringBuilder.Append(formatItem(formatter, item, options).Indent("  ", false));
		}

		if (hasMoreValues)
		{
			const char ellipsis = '\u2026';
			stringBuilder.Append(ellipsis);
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
		FormattingOptions options)
		=> Format(formatter, item, options with
		{
			IncludeType = false,
		});

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
