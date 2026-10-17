using System.Collections.Generic;
using System.Text;

namespace aweXpect.Formatting;

/// <summary>
///     Extension formatting options.
/// </summary>
public static partial class ValueFormatters
{
	/// <summary>
	///     Returns the formatted <paramref name="value" /> according to the <paramref name="options" />.
	/// </summary>
	public static string Format<TKey, TValue>(
		this ValueFormatter formatter,
		KeyValuePair<TKey, TValue> value,
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
	public static void Format<TKey, TValue>(
		this ValueFormatter formatter,
		StringBuilder stringBuilder,
		KeyValuePair<TKey, TValue> value,
		FormattingOptions? options = null)
	{
		if (TryFormatWithRegistrations(stringBuilder, value, options))
		{
			return;
		}

		if (options?.IncludeType == true)
		{
			formatter.Format(stringBuilder, typeof(KeyValuePair<TKey, TValue>));
			stringBuilder.Append(' ');
			options = options with
			{
				IncludeType = false,
			};
		}

		AppendKeyValuePair(formatter, stringBuilder, value.Key, value.Value, options, null);
	}

	/// <remarks>
	///     When <see cref="FormattingOptions.FormatValue" /> is set, the key of a dictionary <paramref name="isEntry" />
	///     is not offered, as it is part of the path, and the value is offered under the path of the entry. A pair that
	///     is no entry is compared by its members in an equivalency comparison, so its key and value are offered under
	///     the paths of its <c>Key</c> and <c>Value</c>.
	/// </remarks>
	private static void AppendKeyValuePair(
		ValueFormatter formatter,
		StringBuilder stringBuilder,
		object? key,
		object? value,
		FormattingOptions? options,
		FormattingContext? context,
		bool isEntry = false)
	{
		if (options?.FormatValue is null)
		{
			stringBuilder.Append('[');
			formatter.Format(stringBuilder, key, WithoutLineBreaksForString(key, options), context);
			stringBuilder.Append("] = ");
			formatter.Format(stringBuilder, value, WithoutLineBreaksForString(value, options), context);
			return;
		}

		context ??= new FormattingContext();
		FormattedValue? parent = context.Value;
		stringBuilder.Append('[');
		if (isEntry)
		{
			formatter.Format(stringBuilder, key, WithoutLineBreaksForString(key, WithoutFormatValue(options)),
				context);
			stringBuilder.Append("] = ");
			formatter.Format(stringBuilder, value, WithoutLineBreaksForString(value, options), context);
			return;
		}

		try
		{
			context.Value = FormattedValue.Segment(parent, nameof(KeyValuePair<object, object>.Key));
			formatter.Format(stringBuilder, key, WithoutLineBreaksForString(key, options), context);
			stringBuilder.Append("] = ");
			context.Value = FormattedValue.Segment(parent, nameof(KeyValuePair<object, object>.Value));
			formatter.Format(stringBuilder, value, WithoutLineBreaksForString(value, options), context);
		}
		finally
		{
			context.Value = parent;
		}
	}

	/// <remarks>
	///     A string is only escaped and truncated on a single line, so a nested one is kept on a single line like a
	///     collection item or an object member.
	/// </remarks>
	private static FormattingOptions? WithoutLineBreaksForString(object? value, FormattingOptions? options)
		=> value is string && options?.UseLineBreaks == true
			? options with
			{
				UseLineBreaks = false,
			}
			: options;
}
