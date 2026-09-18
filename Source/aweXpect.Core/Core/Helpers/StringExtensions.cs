using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace aweXpect.Core.Helpers;

internal static class StringExtensions
{
	[return: NotNullIfNotNull(nameof(value))]
	public static string? DisplayWhitespace(this string? value) =>
		value?.Replace("\n", "\\n").Replace("\r", "\\r").Replace("\t", "\\t");

	[return: NotNullIfNotNull(nameof(value))]
	public static string? Indent(this string? value, string? indentation = "  ",
		bool indentFirstLine = true)
	{
		if (value == null || string.IsNullOrEmpty(indentation))
		{
			return value;
		}

		return (indentFirstLine ? indentation : "")
		       + value.Replace("\n", $"\n{indentation}");
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

	/// <summary>
	///     Removes the leading white-space from every line and normalizes the newline style to <c>\n</c>.
	/// </summary>
	[return: NotNullIfNotNull(nameof(value))]
	public static string? RemoveIndentation(this string? value)
	{
		if (value == null)
		{
			return null;
		}

		return string.Join("\n", value.RemoveNewlineStyle().Split('\n').Select(line => line.TrimStart()));
	}

	[return: NotNullIfNotNull(nameof(value))]
	public static string? RemoveNewlineStyle(this string? value)
	{
		if (value == null)
		{
			return null;
		}

		return value.Replace("\r\n", "\n", StringComparison.Ordinal)
			.Replace("\r", "\n", StringComparison.Ordinal);
	}

	public static string SubstringUntilFirst(this string name, char c)
	{
		int index = name.IndexOf(c);
		if (index >= 0)
		{
			return name.Substring(0, index);
		}

		return name;
	}

	[return: NotNullIfNotNull(nameof(value))]
	public static string? ToSingleLine(this string? value)
		=> value?.Replace("\n", "\\n").Replace("\r", "\\r");

	[return: NotNullIfNotNull(nameof(value))]
	public static string? TruncateWithEllipsis(this string? value, int maxLength)
	{
		if (value is null || value.Length <= maxLength)
		{
			return value;
		}

		const char ellipsis = '\u2026';
		return $"{value.Substring(0, maxLength)}{ellipsis}";
	}

	[return: NotNullIfNotNull(nameof(value))]
	public static string? TruncateWithEllipsisOnWord(this string? value, int maxLength)
	{
		if (value is null || value.Length <= maxLength)
		{
			return value;
		}

		int indexOfWordBoundary = value[..maxLength].LastIndexOf(' ');
		if (indexOfWordBoundary < maxLength * 0.8)
		{
			indexOfWordBoundary = maxLength;
		}

		const char ellipsis = '\u2026';
		return $"{value.Substring(0, indexOfWordBoundary)}{ellipsis}";
	}

	public static string TrimCommonWhiteSpace(this string value)
	{
		string[] lines = value.Split('\n');
		if (lines.Length <= 1)
		{
			return value;
		}

		StringBuilder sb = new();
		foreach (char c in lines[1])
		{
			if (char.IsWhiteSpace(c))
			{
				sb.Append(c);
			}
			else
			{
				break;
			}
		}

		string commonWhiteSpace = sb.ToString();

		for (int l = 2; l < lines.Length; l++)
		{
			if (lines[l].StartsWith(commonWhiteSpace))
			{
				continue;
			}

			for (int i = 0; i < Math.Min(lines[l].Length, commonWhiteSpace.Length); i++)
			{
				if (lines[l][i] != commonWhiteSpace[i])
				{
					commonWhiteSpace = commonWhiteSpace[..i];
					break;
				}
			}
		}

		sb.Clear();
		sb.Append(lines[0]);
		foreach (string? line in lines.Skip(1))
		{
			sb.Append('\n').Append(line[commonWhiteSpace.Length..]);
		}

		return sb.ToString();
	}
}
