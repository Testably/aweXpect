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
				FormatItems(stringBuilder, value, value.Cast<object?>(), dictionary.Count, options,
					new ItemFormatter<object?>(formatter, context, FormatDictionaryItem));
			}
			else
			{
				FormatItems(stringBuilder, value, GetEntries(dictionary), dictionary.Count, options,
					new ItemFormatter<DictionaryEntry>(formatter, context, FormatDictionaryEntry));
			}
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
			Format(Formatter, stringBuilder, value.GetType());
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
}
