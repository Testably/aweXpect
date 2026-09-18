using System;
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
	///     An exception is formatted as its type name and message, without namespace, inner exceptions or stack trace.
	/// </remarks>
	public static string Format(
		this ValueFormatter formatter,
		Exception? value,
		FormattingOptions? options = null)
	{
		StringBuilder stringBuilder = new();
		Format(formatter, stringBuilder, value, options);
		return stringBuilder.ToString();
	}

	/// <summary>
	///     Appends the formatted <paramref name="value" /> according to the <paramref name="options" />
	///     to the <paramref name="stringBuilder" />
	/// </summary>
	/// <remarks>
	///     An exception is formatted as its type name and message, without namespace, inner exceptions or stack trace.
	/// </remarks>
	public static void Format(
		this ValueFormatter formatter,
		StringBuilder stringBuilder,
		Exception? value,
		FormattingOptions? options = null)
	{
		if (value == null)
		{
			stringBuilder.Append(ValueFormatter.NullString);
			return;
		}

		FormatType(value.GetType(), stringBuilder);
		string? message = value.Message;
		if (string.IsNullOrEmpty(message))
		{
			return;
		}

		stringBuilder.Append(": ");
		options ??= FormattingOptions.SingleLine;
		if (options.UseLineBreaks)
		{
			stringBuilder.Append(message.Indent(indentFirstLine: false));
		}
		else
		{
			stringBuilder.Append(message.DisplayWhitespace()
				.TruncateWithEllipsis(Customize.aweXpect.Formatting().MaximumStringLength.Get()));
		}
	}
}
