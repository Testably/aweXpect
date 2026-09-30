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
		Format(formatter, stringBuilder, value, options);
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

		if (options is not null)
		{
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
		Format(formatter, stringBuilder, key, options, context);
		stringBuilder.Append("] = ");
		Format(formatter, stringBuilder, value, options, context);
	}
}
