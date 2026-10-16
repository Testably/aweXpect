using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;

namespace aweXpect.Core.Helpers;

internal static class StringExtensions
{
	/// <remarks>
	///     Without globalization data, like in the invariant globalization mode, every string counts as normalized.
	/// </remarks>
	private static readonly bool CanCheckNormalization = !"e\u0301".IsNormalized();

	/// <summary>
	///     Counts the <c>\n</c> characters in the first <paramref name="length" /> characters of the
	///     <paramref name="value" />.
	/// </summary>
	public static int CountLineFeeds(this string value, int length)
	{
		int count = 0;
		int index = value.IndexOf('\n', 0, length);
		while (index >= 0)
		{
			count++;
			index = value.IndexOf('\n', index + 1, length - index - 1);
		}

		return count;
	}

	/// <summary>
	///     Makes line breaks, tabs, control characters and invisible characters in unquoted text visible as escape
	///     sequences (<c>\n</c>, <c>\r</c>, <c>\t</c>, <c>\0</c> or <c>\uXXXX</c>).
	/// </summary>
	/// <remarks>
	///     Invisible characters are format characters (like a zero-width space) and separators other than the plain
	///     space (like a non-breaking space). A combining mark is escaped at the start, where it would combine with the
	///     preceding text, and in a value that is not normalized, where it can make a decomposed letter look like its
	///     precomposed one; in normalized text, like most Hindi or Thai text, it stays readable. An unpaired surrogate is
	///     escaped as well, as it is no valid text and cannot be written by a strict encoder. Backslashes are kept, as
	///     unquoted text, like an exception message, is not read as a literal and often holds a path.
	/// </remarks>
	[return: NotNullIfNotNull(nameof(value))]
	public static string? DisplayWhitespace(this string? value) => value.Escape(null);

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
		bool? escapesCombiningMarks = null;
		for (int index = 0; index < value.Length; index++)
		{
			char c = value[index];
			if (!NeedsEscaping(value, index, quote, ref escapesCombiningMarks))
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

		static bool NeedsEscaping(string value, int index, char? quote, ref bool? escapesCombiningMarks)
		{
			char c = value[index];
			if (c == ' ')
			{
				return false;
			}

			if (quote is not null && (c == '\\' || c == quote))
			{
				return true;
			}

			return CharUnicodeInfo.GetUnicodeCategory(c) switch
			{
				UnicodeCategory.Surrogate => !char.IsSurrogatePair(value, index) &&
				                             !(index > 0 && char.IsSurrogatePair(value, index - 1)),
				UnicodeCategory.NonSpacingMark => index == 0 || (escapesCombiningMarks ??= !IsNormalized(value)),
				UnicodeCategory.Control or UnicodeCategory.Format or UnicodeCategory.SpaceSeparator
					or UnicodeCategory.LineSeparator or UnicodeCategory.ParagraphSeparator => true,
				_ => false,
			};
		}
	}

	/// <summary>
	///     Checks if the <paramref name="value" /> is in the Unicode normalization form C, in which a combining mark only
	///     remains where no precomposed character exists.
	/// </summary>
	/// <remarks>
	///     A value whose normalization cannot be checked, or that is no valid text, counts as not normalized.
	/// </remarks>
	private static bool IsNormalized(string value)
	{
		try
		{
			return CanCheckNormalization && value.IsNormalized();
		}
		catch (ArgumentException)
		{
			return false;
		}
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

		string normalized = value.RemoveNewlineStyle();
		if (!HasIndentedLine(normalized))
		{
			return normalized;
		}

		return string.Join("\n", normalized.Split('\n').Select(line => line.TrimStart()));
	}

	private static bool HasIndentedLine(string value)
	{
		int lineStart = 0;
		while (lineStart < value.Length)
		{
			if (char.IsWhiteSpace(value[lineStart]))
			{
				return true;
			}

			int newline = value.IndexOf('\n', lineStart);
			if (newline < 0)
			{
				return false;
			}

			lineStart = newline + 1;
		}

		return false;
	}

	[return: NotNullIfNotNull(nameof(value))]
	public static string? RemoveNewlineStyle(this string? value)
	{
		if (value == null)
		{
			return null;
		}

		if (value.IndexOf('\r') < 0)
		{
			return value;
		}

		return value.Replace("\r\n", "\n", StringComparison.Ordinal)
			.Replace("\r", "\n", StringComparison.Ordinal);
	}

	/// <summary>
	///     Replaces every run of whitespace, including line breaks, with a single space.
	/// </summary>
	/// <remarks>
	///     A value in which every whitespace is already a single space is returned unchanged, without allocating.
	/// </remarks>
	[return: NotNullIfNotNull(nameof(value))]
	public static string? CollapseWhiteSpace(this string? value)
	{
		if (value == null || !HasWhiteSpaceToCollapse(value))
		{
			return value;
		}

		StringBuilder sb = new(value.Length);
		bool isInWhiteSpace = false;
		foreach (char c in value)
		{
			if (!char.IsWhiteSpace(c))
			{
				sb.Append(c);
				isInWhiteSpace = false;
			}
			else if (!isInWhiteSpace)
			{
				sb.Append(' ');
				isInWhiteSpace = true;
			}
		}

		return sb.ToString();
	}

	private static bool HasWhiteSpaceToCollapse(string value)
	{
		for (int i = 0; i < value.Length; i++)
		{
			if (char.IsWhiteSpace(value[i]) &&
			    (value[i] != ' ' || (i + 1 < value.Length && char.IsWhiteSpace(value[i + 1]))))
			{
				return true;
			}
		}

		return false;
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
		=> value.IsSplitAt(index) ? index - 1 : index;

	/// <summary>
	///     Checks if cutting the <paramref name="value" /> at the <paramref name="index" /> would split a surrogate pair
	///     or a <c>\r\n</c> line break.
	/// </summary>
	public static bool IsSplitAt(this string value, int index)
		=> index > 0 && index < value.Length &&
		   ((char.IsHighSurrogate(value[index - 1]) && char.IsLowSurrogate(value[index])) ||
		    (value[index - 1] == '\r' && value[index] == '\n'));
}
