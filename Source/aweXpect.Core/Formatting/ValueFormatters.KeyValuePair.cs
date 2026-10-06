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

	private static void AppendKeyValuePair(
		ValueFormatter formatter,
		StringBuilder stringBuilder,
		object? key,
		object? value,
		FormattingOptions? options,
		FormattingContext? context)
	{
		stringBuilder.Append('[');
		formatter.Format(stringBuilder, key, WithoutLineBreaksForString(key, options), context);
		stringBuilder.Append("] = ");
		formatter.Format(stringBuilder, value, WithoutLineBreaksForString(value, options), context);
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
