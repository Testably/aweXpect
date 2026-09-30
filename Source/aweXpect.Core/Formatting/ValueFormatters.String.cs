using System.Text;
using aweXpect.Core.Helpers;
using aweXpect.Customization;

namespace aweXpect.Formatting;

public static partial class ValueFormatters
{
	/// <summary>
	///     Returns the formatted <paramref name="value" /> according to the <paramref name="options" />.
	/// </summary>
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

		options ??= FormattingOptions.SingleLine;
		return (options.UseLineBreaks, options.IncludeType) switch
		{
			(true, true) => $"string \"{value}\"",
			(false, true) => $"string \"{value.DisplayWhitespace().TruncateWithEllipsis(Customize.aweXpect.Formatting().MaximumStringLength.Get())}\"",
			(true, false) => $"\"{value}\"",
			(false, false) => $"\"{value.DisplayWhitespace().TruncateWithEllipsis(Customize.aweXpect.Formatting().MaximumStringLength.Get())}\"",
		};
	}

	/// <summary>
	///     Appends the formatted <paramref name="value" /> according to the <paramref name="options" />
	///     to the <paramref name="stringBuilder" />.
	/// </summary>
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

		options ??= FormattingOptions.SingleLine;
		if (options.IncludeType)
		{
			stringBuilder.Append("string ");
		}

		stringBuilder.Append('\"');
		if (!options.UseLineBreaks)
		{
			stringBuilder.Append(value.DisplayWhitespace().TruncateWithEllipsis(Customize.aweXpect.Formatting().MaximumStringLength.Get()));
		}
		else
		{
			stringBuilder.Append(value);
		}

		stringBuilder.Append('\"');
	}
}
