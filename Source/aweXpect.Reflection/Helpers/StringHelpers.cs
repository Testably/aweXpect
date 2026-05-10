using System;
using System.Linq;
using System.Text;

namespace aweXpect.Reflection.Helpers;

internal static class StringHelpers
{
	public static string PrefixIn(this string description)
	{
		if (description.StartsWith("in "))
		{
			return description;
		}

		return "in " + description;
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
					commonWhiteSpace = commonWhiteSpace.Substring(0, i);
					break;
				}
			}
		}

		sb.Clear();
		sb.Append(lines[0]);
		foreach (string? line in lines.Skip(1))
		{
#if NET8_0_OR_GREATER
			sb.Append('\n').Append(line.AsSpan(commonWhiteSpace.Length));
#else
			sb.Append('\n').Append(line.Substring(commonWhiteSpace.Length));
#endif
		}

		if (lines.Length == 2)
		{
			sb.Replace(Environment.NewLine, " ");
		}

		return sb.ToString();
	}
}
