using System;
using System.Collections.Generic;
using System.Globalization;

namespace aweXpect.Helpers;

internal static class StringExtensions
{
	/// <summary>
	///     Checks if the <paramref name="value" /> contains a character of the <paramref name="first" /> or the
	///     <paramref name="second" /> category.
	/// </summary>
	/// <remarks>
	///     A surrogate pair is categorized as the single character it encodes.
	/// </remarks>
	public static bool ContainsCharacterOfCategory(this string value, UnicodeCategory first, UnicodeCategory second)
	{
		for (int i = 0; i < value.Length; i++)
		{
			UnicodeCategory category = CharUnicodeInfo.GetUnicodeCategory(value, i);
			if (category == first || category == second)
			{
				return true;
			}

			if (char.IsSurrogatePair(value, i))
			{
				i++;
			}
		}

		return false;
	}

	/// <summary>
	///     Counts the lines of the <paramref name="value" />, separated by <c>\r\n</c>, <c>\n</c> or <c>\r</c>.
	/// </summary>
	/// <remarks>
	///     A single trailing line terminator does not start a new line, so <c>"a\nb\n"</c> has two lines.
	/// </remarks>
	public static int GetLineCount(this string? value)
	{
		if (string.IsNullOrEmpty(value))
		{
			return 0;
		}

		int count = 1;
		for (int i = 0; i < value!.Length; i++)
		{
			if (value[i] == '\n')
			{
				count++;
			}
			else if (value[i] == '\r')
			{
				count++;
				// "\r\n" is a single separator.
				if (i + 1 < value.Length && value[i + 1] == '\n')
				{
					i++;
				}
			}
		}

		if (value[value.Length - 1] is '\n' or '\r')
		{
			count--;
		}

		return count;
	}

	/// <summary>
	///     Splits the <paramref name="value" /> into lines, separated by <c>\r\n</c>, <c>\n</c> or <c>\r</c>.
	/// </summary>
	/// <remarks>
	///     A single trailing line terminator does not start a new line, so <c>"a\nb\n"</c> has two lines.
	/// </remarks>
	public static IEnumerable<string> GetLines(this string? value)
	{
		if (string.IsNullOrEmpty(value))
		{
			return [];
		}

		// Split matches the earliest separator and, on ties, the first one listed,
		// so "\r\n" is never split into two lines.
		string[] lines = value!.Split(["\r\n", "\n", "\r",], StringSplitOptions.None);
		// The lines are returned as a collection, so that expectations on them know the number of lines up front.
		if (lines[lines.Length - 1].Length == 0)
		{
			Array.Resize(ref lines, lines.Length - 1);
		}

		return lines;
	}
}
