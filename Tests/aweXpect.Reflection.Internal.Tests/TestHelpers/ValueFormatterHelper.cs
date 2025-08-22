using System.Text;
using aweXpect.Formatting;

namespace aweXpect.Reflection.Internal.Tests.TestHelpers;

internal static class ValueFormatterHelpers
{
	public static string GetString<T>(this IValueFormatter formatter, T value, FormattingOptions? options = null)
	{
		StringBuilder sb = new();
		if (!formatter.TryFormat(sb, value!, options))
		{
			throw new NotSupportedException($"Formatter doesn't support formatting of type {typeof(T)}");
		}

		return sb.ToString();
	}
}
