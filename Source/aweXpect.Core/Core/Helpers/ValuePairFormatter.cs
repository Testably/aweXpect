using System;
using aweXpect.Customization;

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
	///     <para />
	///     Two strings that only differ after the maximum string length are truncated to the same text, so they are
	///     shown from shortly before their first difference instead.
	/// </remarks>
	public static (string Actual, string Expected) Format(object? actual, object? expected,
		FormattingOptions? options = null)
	{
		string actualText = Formatter.Format(actual, options);
		string expectedText = Formatter.Format(expected, options);
		if (!string.Equals(actualText, expectedText, StringComparison.Ordinal))
		{
			return (actualText, expectedText);
		}

		if (actual is string actualString && expected is string expectedString)
		{
			const char ellipsis = '…';
			int start = GetStartBeforeFirstDifference(actualString, expectedString);
			return start == 0
				? (actualText, expectedText)
				: (Formatter.Format(ellipsis + actualString.Substring(start), options),
					Formatter.Format(ellipsis + expectedString.Substring(start), options));
		}

		if (actual?.GetType() == expected?.GetType())
		{
			return (actualText, expectedText);
		}

		return (AppendRuntimeType(actualText, actual), AppendRuntimeType(expectedText, expected));
	}

	/// <summary>
	///     Appends the runtime type of the <paramref name="value" /> to its formatted <paramref name="text" />.
	/// </summary>
	public static string AppendRuntimeType(string text, object? value)
		=> value is null ? text : $"{text} ({Formatter.Format(value.GetType())})";

	/// <summary>
	///     Gets the index a few characters before the first difference of <paramref name="actual" /> and
	///     <paramref name="expected" />, so that the difference stays within the maximum string length after a leading
	///     ellipsis.
	/// </summary>
	private static int GetStartBeforeFirstDifference(string actual, string expected)
	{
		const int charactersBeforeDifference = 10;
		int maxStringLength = Customize.aweXpect.Formatting().MaximumStringLength.Get();
		int minLength = Math.Min(actual.Length, expected.Length);
		int indexOfFirstDifference = 0;
		while (indexOfFirstDifference < minLength &&
		       actual[indexOfFirstDifference] == expected[indexOfFirstDifference])
		{
			indexOfFirstDifference++;
		}

		int start = Math.Max(0, indexOfFirstDifference - Math.Max(0, Math.Min(charactersBeforeDifference, maxStringLength - 2)));
		return actual.IsSplitAt(start) || expected.IsSplitAt(start) ? start - 1 : start;
	}
}
