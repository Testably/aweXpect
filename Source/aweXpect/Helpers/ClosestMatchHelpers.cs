using System;
using System.Collections.Generic;
using aweXpect.Core.Extending;
using aweXpect.Customization;

namespace aweXpect.Helpers;

/// <summary>
///     Suggests the string of the subject that was most likely meant, when an expected string was not found.
/// </summary>
internal static class ClosestMatchHelpers
{
	/// <summary>
	///     Appends the hint of <see cref="ClosestMatch.AppendHint(StringBuilder, string, IEnumerable{string})" /> when
	///     the <paramref name="value" /> is a <see langword="string" /> and one of the <paramref name="candidates" /> is
	///     close to it.
	/// </summary>
	public static void AppendClosestMatchHint<T>(this StringBuilder stringBuilder, T value,
		IEnumerable<T>? candidates)
	{
		if (value is string expected && candidates is IEnumerable<string> strings)
		{
			try
			{
				ClosestMatch.AppendHint(stringBuilder, expected, strings);
			}
			catch (Exception)
			{
				// The failure has to be reported anyway, so a subject whose enumeration throws suggests nothing.
			}
		}
	}

	/// <summary>
	///     Appends the <paramref name="values" /> on multiple lines like the <see cref="Formatter" />, each followed by
	///     the hint of <see cref="AppendClosestMatchHint{T}" />.
	/// </summary>
	public static void AppendWithClosestMatchHints<T>(this StringBuilder stringBuilder, List<T>? values,
		IEnumerable<T>? candidates, string? indentation)
	{
		if (values is not List<string> strings || candidates is not IEnumerable<string> stringCandidates)
		{
			stringBuilder.Append(Formatter.Format(values, FormattingOptions.MultipleLines).Indent(indentation, false));
			return;
		}

		int maximumNumberOfItems = Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Get();
		StringBuilder list = new();
		list.Append('[');
		for (int index = 0; index < strings.Count; index++)
		{
			list.AppendLine(index == 0 ? "" : ",").Append("  ");
			if (index == maximumNumberOfItems)
			{
				list.Append("(… and ").Append(strings.Count - maximumNumberOfItems).Append(" more)");
				break;
			}

			list.Append(Formatter.Format(strings[index], FormattingOptions.MultipleLines));
			list.AppendClosestMatchHint(strings[index], stringCandidates);
		}

		list.AppendLine().Append(']');
		stringBuilder.Append(list.ToString().Indent(indentation, false));
	}
}
