using System;

namespace aweXpect.Core.Helpers;

internal static class ValuePairFormatter
{
	/// <summary>
	///     Formats the <paramref name="actual" /> and <paramref name="expected" /> value of a failure message with the
	///     <paramref name="options" />.
	/// </summary>
	/// <remarks>
	///     Two values that format identically leave the reader with a message that shows no difference at all, so the
	///     runtime type is appended to both to name what sets them apart. It is omitted whenever the formatted values
	///     already differ, where it would be noise, and whenever both values have the same type, where it would not
	///     tell them apart either.
	/// </remarks>
	public static (string Actual, string Expected) Format(object? actual, object? expected,
		FormattingOptions? options = null)
	{
		string actualText = Formatter.Format(actual, options);
		string expectedText = Formatter.Format(expected, options);
		if (!string.Equals(actualText, expectedText, StringComparison.Ordinal) ||
		    actual?.GetType() == expected?.GetType())
		{
			return (actualText, expectedText);
		}

		return (AppendRuntimeType(actualText, actual), AppendRuntimeType(expectedText, expected));
	}

	private static string AppendRuntimeType(string text, object? value)
		=> value is null ? text : $"{text} ({Formatter.Format(value.GetType())})";
}
