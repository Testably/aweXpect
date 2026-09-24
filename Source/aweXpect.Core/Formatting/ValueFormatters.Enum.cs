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

		if (options?.IncludeType == true)
		{
			return $"{Format(formatter, value.GetType())} {value}";
		}

		return value.ToString();
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
		else
		{
			if (options?.IncludeType == true)
			{
				Format(formatter, stringBuilder, value.GetType());
				stringBuilder.Append(' ');
			}

			stringBuilder.Append(value);
		}
	}
}
