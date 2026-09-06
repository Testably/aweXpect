using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace aweXpect.Helpers;

internal static class StringExtensions
{
	/// <summary>
	///     Splits the <paramref name="value" /> into lines, separated by <c>\r\n</c>, <c>\n</c> or <c>\r</c>.
	/// </summary>
	/// <remarks>
	///     A single trailing line terminator does not start a new line, so <c>"a\nb\n"</c> has two lines.
	/// </remarks>
	public static IEnumerable<string?> GetLines(this string? value)
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

	public static string PrependAOrAn(this string value)
	{
		char[] vocals = ['a', 'e', 'i', 'o', 'u', 'A', 'E', 'I', 'O', 'U',];
		if (value.Length > 0 && vocals.Contains(value[0]))
		{
			return $"an {value}";
		}

		return $"a {value}";
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
