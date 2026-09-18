using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace aweXpect.Helpers;

internal static class StringExtensions
{
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
		int count = lines.Length;
		if (lines[count - 1].Length == 0)
		{
			count--;
		}

		return lines.Take(count);
	}

	[return: NotNullIfNotNull(nameof(value))]
	public static string? Indent(this string? value, string? indentation = "  ",
		bool indentFirstLine = true)
	{
		if (value == null)
		{
			return null;
		}

		if (!string.IsNullOrEmpty(indentation))
		{
			return (indentFirstLine ? indentation : "")
			       + value.Replace("\n", $"\n{indentation}");
		}

		return value;
	}

	/// <summary>
	///     Prepends the indefinite article that matches the sound of the first letter of the <paramref name="value" />.
	/// </summary>
	/// <remarks>
	///     An initialism (an uppercase letter that stands alone or is followed by an uppercase letter or a digit, e.g.
	///     "HResult", "IOException" or "UInt32") is read letter by letter, so it takes "an" when the name of its
	///     first letter starts with a vowel sound (A, E, F, H, I, L, M, N, O, R, S, X).<br />
	///     Any other value takes "an" when it starts with a vowel, except for a "U" followed by a single consonant other
	///     than "n" and a vowel, which is read as "you" (e.g. "User", "Uri" or "Utility").
	/// </remarks>
	public static string PrependAOrAn(this string value)
	{
		bool startsWithVowelSound;
		if (value.Length > 0 && char.IsUpper(value[0]) &&
		    (value.Length == 1 || char.IsUpper(value[1]) || char.IsDigit(value[1])))
		{
			startsWithVowelSound =
				value[0] is 'A' or 'E' or 'F' or 'H' or 'I' or 'L' or 'M' or 'N' or 'O' or 'R' or 'S' or 'X';
		}
		else if (value.Length > 2 && value[0] is 'U' or 'u' && value[1] != 'n' && !IsVowel(value[1]) &&
		         IsVowel(value[2]))
		{
			startsWithVowelSound = false;
		}
		else
		{
			startsWithVowelSound = value.Length > 0 && IsVowel(value[0]);
		}

		return startsWithVowelSound ? $"an {value}" : $"a {value}";

		static bool IsVowel(char c) => c is 'a' or 'e' or 'i' or 'o' or 'u' or 'A' or 'E' or 'I' or 'O' or 'U';
	}

	public static string TrimCommonWhiteSpace(this string value)
	{
		string[] lines = value.Split(Environment.NewLine);
		if (lines.Length <= 1)
		{
			return value;
		}

		StringBuilder sb = new();
		foreach (char c in lines[1].TakeWhile(char.IsWhiteSpace))
		{
			sb.Append(c);
		}

		string commonWhiteSpace = sb.ToString();

		foreach (string line in lines.Skip(2).Where(line => !line.StartsWith(commonWhiteSpace)))
		{
			for (int i = 0; i < Math.Min(line.Length, commonWhiteSpace.Length); i++)
			{
				if (line[i] != commonWhiteSpace[i])
				{
					commonWhiteSpace = commonWhiteSpace[..i];
				}
			}
		}

		sb.Clear();
		sb.Append(lines[0]);
		foreach (string? line in lines.Skip(1))
		{
			sb.Append(Environment.NewLine).Append(line[commonWhiteSpace.Length..]);
		}

		return sb.ToString();
	}
}
