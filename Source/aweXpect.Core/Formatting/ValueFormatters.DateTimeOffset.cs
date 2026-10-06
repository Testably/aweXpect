using System;
using System.Text;

namespace aweXpect.Formatting;

public static partial class ValueFormatters
{
	/// <summary>
	///     Returns the formatted <paramref name="value" /> according to the <paramref name="options" />.
	/// </summary>
	public static string Format(
		this ValueFormatter formatter,
		DateTimeOffset value,
		FormattingOptions? options = null)
		=> TryFormatWithRegistrations(value, options, out string? customValue)
			? customValue
			: options?.IncludeType switch
			{
				true => $"DateTimeOffset {value:o}",
				_ => value.ToString("o"),
			};

	/// <summary>
	///     Appends the formatted <paramref name="value" /> according to the <paramref name="options" />
	///     to the <paramref name="stringBuilder" />.
	/// </summary>
	public static void Format(
		this ValueFormatter formatter,
		StringBuilder stringBuilder,
		DateTimeOffset value,
		FormattingOptions? options = null)
	{
		if (TryFormatWithRegistrations(stringBuilder, value, options))
		{
			return;
		}

		if (options?.IncludeType == true)
		{
			stringBuilder.Append("DateTimeOffset ");
		}

		stringBuilder.Append(value.ToString("o"));
	}

	/// <summary>
	///     Returns the formatted <paramref name="value" /> according to the <paramref name="options" />.
	/// </summary>
	public static string Format(
		this ValueFormatter formatter,
		DateTimeOffset? value,
		FormattingOptions? options = null)
	{
		if (value == null)
		{
			return ValueFormatter.NullString;
		}

		return formatter.Format(value.Value, options);
	}

	/// <summary>
	///     Appends the formatted <paramref name="value" /> according to the <paramref name="options" />
	///     to the <paramref name="stringBuilder" />.
	/// </summary>
	public static void Format(
		this ValueFormatter formatter,
		StringBuilder stringBuilder,
		DateTimeOffset? value,
		FormattingOptions? options = null)
	{
		if (value == null)
		{
			stringBuilder.Append(ValueFormatter.NullString);
			return;
		}

		formatter.Format(stringBuilder, value.Value, options);
	}
}
