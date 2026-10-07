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
		Enum? value,
		FormattingOptions? options = null)
	{
		if (value == null)
		{
			return ValueFormatter.NullString;
		}

		if (TryFormatWithRegistrations(value, options, out string? customValue))
		{
			return customValue;
		}

		if (options?.IncludeType == true)
		{
			return $"{formatter.Format(value.GetType())} {FormatEnumValue(value)}";
		}

		return FormatEnumValue(value);
	}

	/// <summary>
	///     Appends the formatted <paramref name="value" /> according to the <paramref name="options" />
	///     to the <paramref name="stringBuilder" />.
	/// </summary>
	public static void Format(
		this ValueFormatter formatter,
		StringBuilder stringBuilder,
		Enum? value,
		FormattingOptions? options = null)
	{
		if (value == null)
		{
			stringBuilder.Append(ValueFormatter.NullString);
		}
		else if (!TryFormatWithRegistrations(stringBuilder, value, options))
		{
			if (options?.IncludeType == true)
			{
				formatter.Format(stringBuilder, value.GetType());
				stringBuilder.Append(' ');
			}

			stringBuilder.Append(FormatEnumValue(value));
		}
	}

	/// <remarks>
	///     A combination of flags is joined like in C#, because the comma of <see cref="Enum.ToString()" /> reads like the
	///     separator of collection items.
	/// </remarks>
	private static string FormatEnumValue(Enum value)
		=> value.ToString().Replace(", ", " | ");
}
