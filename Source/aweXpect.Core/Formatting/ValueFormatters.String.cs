using System.Text;
using aweXpect.Core.Helpers;
using aweXpect.Customization;

namespace aweXpect.Formatting;

public static partial class ValueFormatters
{
	/// <summary>
	///     Returns the formatted <paramref name="value" /> according to the <paramref name="options" />.
	/// </summary>
	/// <remarks>
	///     The value is always escaped and truncated on a single line, also when the <paramref name="options" /> use line
	///     breaks, so that a quote, a line break or a long value cannot break the layout of the message.
	/// </remarks>
	public static string Format(
		this ValueFormatter formatter,
		string? value,
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

		string formattedValue =
			$"\"{value.TruncateWithEllipsis(Customize.aweXpect.Formatting().MaximumStringLength.Get()).Escape()}\"";
		return options?.IncludeType == true ? $"string {formattedValue}" : formattedValue;
	}

	/// <summary>
	///     Appends the formatted <paramref name="value" /> according to the <paramref name="options" />
	///     to the <paramref name="stringBuilder" />.
	/// </summary>
	/// <remarks>
	///     The value is always escaped and truncated on a single line, also when the <paramref name="options" /> use line
	///     breaks, so that a quote, a line break or a long value cannot break the layout of the message.
	/// </remarks>
	public static void Format(
		this ValueFormatter formatter,
		StringBuilder stringBuilder,
		string? value,
		FormattingOptions? options = null)
	{
		if (value == null)
		{
			stringBuilder.Append(ValueFormatter.NullString);
			return;
		}

		if (TryFormatWithRegistrations(stringBuilder, value, options))
		{
			return;
		}

		if (options?.IncludeType == true)
		{
			stringBuilder.Append("string ");
		}

		stringBuilder.Append('\"');
		stringBuilder.Append(value.TruncateWithEllipsis(Customize.aweXpect.Formatting().MaximumStringLength.Get()).Escape());
		stringBuilder.Append('\"');
	}
}
