using System;
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

		string name = IsDefined(value.Value) ? $" {value}" : "";
		return options?.IncludeType switch
		{
			true => $"HttpStatusCode {(int)value}{name}",
			_ => $"{(int)value}{name}",
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

		stringBuilder.Append((int)value);
		if (IsDefined(value.Value))
		{
			stringBuilder.Append(' ').Append(value);
		}
	}

	/// <summary>
	///     An undefined value has no name, so <see cref="Enum.ToString()" /> would only repeat its number.
	/// </summary>
	private static bool IsDefined(HttpStatusCode value)
#if NET8_0_OR_GREATER
		=> Enum.IsDefined(value);
#else
		=> Enum.IsDefined(typeof(HttpStatusCode), value);
#endif
}
