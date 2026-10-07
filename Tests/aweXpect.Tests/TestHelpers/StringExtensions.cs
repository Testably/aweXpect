using System.Diagnostics.CodeAnalysis;

namespace aweXpect.Tests;

internal static class StringExtensions
{
	[return: NotNullIfNotNull(nameof(value))]
	public static string? DisplayWhitespace(this string? value)
		=> value?.Replace("\n", "\\n").Replace("\r", "\\r").Replace("\t", "\\t");

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

	public static string ToTimesString(this int value)
		=> value switch
		{
			1 => "once",
			2 => "twice",
			_ => $"{value} times",
		};
}
