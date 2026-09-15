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
		Guid value,
		FormattingOptions? options = null)
		=> options?.IncludeType switch
		{
			true => $"Guid {value}",
			_ => value.ToString(),
		};

	/// <summary>
	///     Appends the formatted <paramref name="value" /> according to the <paramref name="options" />
	///     to the <paramref name="stringBuilder" />
	/// </summary>
	public static void Format(
		this ValueFormatter formatter,
		StringBuilder stringBuilder,
		Guid value,
		FormattingOptions? options = null)
	{
		if (options?.IncludeType == true)
		{
			stringBuilder.Append("Guid ");
		}

		stringBuilder.Append(value);
	}

	/// <summary>
	///     Returns the formatted <paramref name="value" /> according to the <paramref name="options" />.
	/// </summary>
	public static string Format(
		this ValueFormatter formatter,
		Guid? value,
		FormattingOptions? options = null)
	{
		if (value == null)
		{
			return ValueFormatter.NullString;
		}

		return Format(formatter, value.Value, options);
	}

	/// <summary>
	///     Appends the formatted <paramref name="value" /> according to the <paramref name="options" />
	///     to the <paramref name="stringBuilder" />
	/// </summary>
	public static void Format(
		this ValueFormatter formatter,
		StringBuilder stringBuilder,
		Guid? value,
		FormattingOptions? options = null)
	{
		if (value == null)
		{
			stringBuilder.Append(ValueFormatter.NullString);
			return;
		}

		Format(formatter, stringBuilder, value.Value, options);
	}
}
