using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using aweXpect.Core.Helpers;

namespace aweXpect.Core.Extending;

/// <summary>
///     Text helpers for writing the expectation and result texts of a constraint.
/// </summary>
/// <remarks>
///     This class is in its own namespace, so that its methods are only suggested to extension authors who import it.
/// </remarks>
public static class StringExtensions
{
	/// <summary>
	///     Prepends the <paramref name="indentation" /> to every line of the <paramref name="value" />, e.g. to nest a
	///     formatted value or a message in a result text.
	/// </summary>
	/// <param name="value">The text to indent.</param>
	/// <param name="indentation">
	///     The indentation for each line. Pass on the <c>indentation</c> of <c>AppendExpectation</c> or
	///     <c>AppendResult</c>; without indentation (<see langword="null" /> or empty) the <paramref name="value" /> is
	///     returned unchanged.
	/// </param>
	/// <param name="indentFirstLine">
	///     Whether the first line is indented as well. Pass <see langword="false" /> when the <paramref name="value" />
	///     continues a line that is already indented.
	/// </param>
	/// <remarks>
	///     A line starts after each <c>\n</c>, so that the line endings (<c>\n</c> or <c>\r\n</c>) are kept. Empty lines
	///     are indented as well.
	/// </remarks>
	[return: NotNullIfNotNull(nameof(value))]
	public static string? Indent(this string? value, string? indentation = "  ", bool indentFirstLine = true)
	{
		if (value == null || string.IsNullOrEmpty(indentation))
		{
			return value;
		}

		return (indentFirstLine ? indentation : "")
		       + value.Replace("\n", $"\n{indentation}");
	}

	/// <summary>
	///     Prepends the indefinite article that matches the sound of the first letter of the <paramref name="value" />,
	///     e.g. "an ArgumentException" or "a NotSupportedException".
	/// </summary>
	/// <remarks>
	///     An initialism (an uppercase letter that stands alone or is followed by an uppercase letter or a digit, e.g.
	///     "HResult", "IOException" or "UInt32") is read letter by letter, so it takes "an" when the name of its
	///     first letter starts with a vowel sound (A, E, F, H, I, L, M, N, O, R, S, X).<br />
	///     Any other value takes "an" when it starts with a vowel, except for a "U" followed by a single consonant other
	///     than "n" and a vowel, which is read as "you" (e.g. "User", "Uri" or "Utility").
	/// </remarks>
	/// <exception cref="ArgumentNullException">The <paramref name="value" /> is <see langword="null" />.</exception>
	public static string PrependAOrAn(this string value)
	{
		value.ThrowIfNull();
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
	///     Removes the leading whitespace that all lines after the first one have in common, e.g. from a caller
	///     argument expression that spans several lines.
	/// </summary>
	/// <remarks>
	///     The first line is kept as it is, as an expression starts after the code in front of it. The lines are split
	///     on <c>\n</c>, so that the line endings (<c>\n</c> or <c>\r\n</c>) are kept. Blank lines don't limit the
	///     common whitespace, as editors often trim them.
	/// </remarks>
	/// <exception cref="ArgumentNullException">The <paramref name="value" /> is <see langword="null" />.</exception>
	public static string TrimCommonWhiteSpace(this string value)
	{
		value.ThrowIfNull();
		if (value.IndexOf('\n') < 0)
		{
			return value;
		}

		string[] lines = value.Split('\n');

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
