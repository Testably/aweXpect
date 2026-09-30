using System.Text;
using aweXpect.Core.Helpers;

namespace aweXpect.Formatting;

public static partial class ValueFormatters
{
	/// <summary>
	///     Returns the formatted <paramref name="value" /> according to the <paramref name="options" />.
	/// </summary>
	public static string Format(
		this ValueFormatter formatter,
		char value,
		FormattingOptions? options = null)
		=> TryFormatWithRegistrations(value, options, out string? customValue)
			? customValue
			: options?.IncludeType switch
			{
				true => $"char '{value.ToString().Escape('\'')}'",
				_ => $"'{value.ToString().Escape('\'')}'",
			};

	/// <summary>
	///     Appends the formatted <paramref name="value" /> according to the <paramref name="options" />
	///     to the <paramref name="stringBuilder" />.
	/// </summary>
	public static void Format(
		this ValueFormatter formatter,
		StringBuilder stringBuilder,
		char value,
		FormattingOptions? options = null)
	{
		if (TryFormatWithRegistrations(stringBuilder, value, options))
		{
			return;
		}

		if (options?.IncludeType == true)
		{
			stringBuilder.Append("char ");
		}

		stringBuilder.Append('\'');
		stringBuilder.Append(value.ToString().Escape('\''));
		stringBuilder.Append('\'');
	}

	/// <summary>
	///     Returns the formatted <paramref name="value" /> according to the <paramref name="options" />.
	/// </summary>
	public static string Format(
		this ValueFormatter formatter,
		char? value,
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
	///     to the <paramref name="stringBuilder" />.
	/// </summary>
	public static void Format(
		this ValueFormatter formatter,
		StringBuilder stringBuilder,
		char? value,
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
