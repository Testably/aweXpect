using System.Net;
using System.Text;

namespace aweXpect.Formatting;

public static partial class ValueFormatters
{
	/// <summary>
	///     Returns the formatted <paramref name="value" /> according to the <paramref name="options" />.
	/// </summary>
	public static string Format(
		this ValueFormatter formatter,
		HttpStatusCode? value,
		FormattingOptions? options = null)
	{
		if (value == null)
		{
			return ValueFormatter.NullString;
		}

		if (TryFormatWithRegistrations(value.Value, options, out string? customValue))
		{
			return customValue;
		}

		return options?.IncludeType switch
		{
			true => $"HttpStatusCode {(int)value} {value}",
			_ => $"{(int)value} {value}",
		};
	}

	/// <summary>
	///     Appends the formatted <paramref name="value" /> according to the <paramref name="options" />
	///     to the <paramref name="stringBuilder" />.
	/// </summary>
	public static void Format(
		this ValueFormatter formatter,
		StringBuilder stringBuilder,
		HttpStatusCode? value,
		FormattingOptions? options = null)
	{
		if (value == null)
		{
			stringBuilder.Append(ValueFormatter.NullString);
			return;
		}

		if (TryFormatWithRegistrations(stringBuilder, value.Value, options))
		{
			return;
		}

		if (options?.IncludeType == true)
		{
			stringBuilder.Append("HttpStatusCode ");
		}

		stringBuilder.Append((int)value).Append(' ').Append(value);
	}
}
