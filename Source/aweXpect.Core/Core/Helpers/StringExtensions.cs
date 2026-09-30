using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;

namespace aweXpect.Core.Helpers;

internal static class StringExtensions
{
	/// <summary>
	///     Makes line breaks, tabs, control characters and invisible characters in unquoted text visible as escape
	///     sequences (<c>\n</c>, <c>\r</c>, <c>\t</c>, <c>\0</c> or <c>\uXXXX</c>).
	/// </summary>
	/// <remarks>
	///     Invisible characters are format characters (like a zero-width space) and separators other than the plain
	///     space (like a non-breaking space). Backslashes are kept, as unquoted text, like an exception message, is not
	///     read as a literal and often holds a path.
	/// </remarks>
	[return: NotNullIfNotNull(nameof(value))]
	public static string? DisplayWhitespace(this string? value) => Escape(value, null);

	/// <summary>
	///     Escapes the <paramref name="value" /> like a C# literal enclosed in <paramref name="quote" /> characters, so
	///     that a backslash, the <paramref name="quote" /> and every character the <see cref="DisplayWhitespace" />
	///     escapes are unambiguous.
	/// </summary>
	/// <param name="value">The value to escape.</param>
	/// <param name="quote">
	///     The enclosing quote, or <see langword="null" /> for unquoted text, in which backslashes are kept.
	/// </param>
	[return: NotNullIfNotNull(nameof(value))]
	public static string? Escape(this string? value, char? quote = '"')
	{
		if (value is null)
		{
			return null;
		}

		StringBuilder? sb = null;
		for (int index = 0; index < value.Length; index++)
		{
			char c = value[index];
			if (!NeedsEscaping(c, quote))
			{
				sb?.Append(c);
				continue;
			}

			sb ??= new StringBuilder(value.Length + 8).Append(value, 0, index);
			sb.Append(c switch
			{
				'\n' => "\\n",
				'\r' => "\\r",
				'\t' => "\\t",
				'\0' => "\\0",
				_ when c == '\\' || c == quote => "\\" + c,
				_ => "\\u" + ((int)c).ToString("X4", CultureInfo.InvariantCulture),
			});
		}

		return sb?.ToString() ?? value;

		static bool NeedsEscaping(char c, char? quote)
		{
			if (c == ' ')
			{
				return false;
			}

			if (quote is not null && (c == '\\' || c == quote))
			{
				return true;
			}

			return CharUnicodeInfo.GetUnicodeCategory(c) is UnicodeCategory.Control or UnicodeCategory.Format
				or UnicodeCategory.SpaceSeparator or UnicodeCategory.LineSeparator
				or UnicodeCategory.ParagraphSeparator;
		}
	}

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
	///     Removes the leading whitespace from every line and normalizes the newline style to <c>\n</c>.
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
	public static string? TruncateWithEllipsis(this string? value, int maxLength)
	{
		if (value is null || value.Length <= maxLength)
		{
			return value;
		}

		const char ellipsis = '\u2026';
		return $"{value.Substring(0, GetIndexToCutAt(value, maxLength))}{ellipsis}";
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
			indexOfWordBoundary = GetIndexToCutAt(value, maxLength);
		}

		const char ellipsis = '\u2026';
		return $"{value.Substring(0, indexOfWordBoundary)}{ellipsis}";
	}

	/// <summary>
	///     Moves the <paramref name="index" /> one character back, if cutting there would split a surrogate pair or
	///     a <c>\r\n</c> line break.
	/// </summary>
	private static int GetIndexToCutAt(string value, int index)
	{
		if (index > 0 &&
		    ((char.IsHighSurrogate(value[index - 1]) && char.IsLowSurrogate(value[index])) ||
		     (value[index - 1] == '\r' && value[index] == '\n')))
		{
			return index - 1;
		}

		return index;
	}

	/// <summary>
	///     Removes the leading whitespace that all lines after the first one have in common.
	/// </summary>
	/// <remarks>
	///     The lines are split on <c>\n</c>, so that the line endings (<c>\n</c> or <c>\r\n</c>) are kept.
	///     Blank lines don't limit the common whitespace, as editors often trim them.
	///     <para />
	///     Keep in sync with the copy in aweXpect.
	/// </remarks>
	public static string TrimCommonWhiteSpace(this string value)
	{
		string[] lines = value.Split('\n');
		if (lines.Length <= 1)
		{
			return value;
		}

		string? commonWhiteSpace = null;
		foreach (string line in lines.Skip(1).Where(line => !string.IsNullOrWhiteSpace(line)))
		{
			int length = 0;
			while (length < line.Length && char.IsWhiteSpace(line[length]) &&
			       (commonWhiteSpace is null ||
			        (length < commonWhiteSpace.Length && line[length] == commonWhiteSpace[length])))
			{
				length++;
			}

			commonWhiteSpace = line.Substring(0, length);
		}

		commonWhiteSpace ??= "";
		StringBuilder sb = new(lines[0]);
		foreach (string line in lines.Skip(1))
		{
			sb.Append('\n');
			if (line.StartsWith(commonWhiteSpace, StringComparison.Ordinal))
			{
				sb.Append(line, commonWhiteSpace.Length, line.Length - commonWhiteSpace.Length);
			}
			else if (line.EndsWith('\r'))
			{
				sb.Append('\r');
			}
		}

		return sb.ToString();
	}
}
