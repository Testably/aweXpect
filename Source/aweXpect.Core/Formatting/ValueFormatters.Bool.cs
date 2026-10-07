using System.Text;

namespace aweXpect.Formatting;

public static partial class ValueFormatters
{
	/// <summary>
	///     Returns the formatted <paramref name="value" /> according to the <paramref name="options" />.
	/// </summary>
	public static string Format(
		this ValueFormatter formatter,
		bool value,
		FormattingOptions? options = null)
		=> TryFormatWithRegistrations(value, options, out string? customValue)
			? customValue
			: (value, options?.IncludeType) switch
			{
				(true, true) => "bool True",
				(false, true) => "bool False",
				(true, _) => "True",
				(false, _) => "False",
			};

	/// <summary>
	///     Appends the formatted <paramref name="value" /> according to the <paramref name="options" />
	///     to the <paramref name="stringBuilder" />.
	/// </summary>
	public static void Format(
		this ValueFormatter formatter,
		StringBuilder stringBuilder,
		bool value,
		FormattingOptions? options = null)
		=> stringBuilder.Append(formatter.Format(value, options));

	/// <summary>
	///     Returns the formatted <paramref name="value" /> according to the <paramref name="options" />.
	/// </summary>
	public static string Format(
		this ValueFormatter formatter,
		bool? value,
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
		bool? value,
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
