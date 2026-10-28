using System;
using System.Collections.Generic;
using System.Text;
using aweXpect.Customization;

namespace aweXpect.Reflection.Helpers;

/// <summary>
///     Renders the items of a failure as a list with one line per item, limited like the formatter limits a collection.
/// </summary>
internal static class ItemListRenderer
{
	/// <summary>
	///     Formats the <paramref name="items" /> as a list with one line per item, which <paramref name="appendItem" />
	///     writes, but at most <see cref="AwexpectCustomization.FormattingCustomization.MaximumNumberOfCollectionItems" />
	///     of them, followed by the number of items that are not listed.
	/// </summary>
	public static string Format<T>(IReadOnlyList<T> items, Action<StringBuilder, T> appendItem)
		=> Format(items, null, appendItem);

	/// <inheritdoc cref="Format{T}(IReadOnlyList{T}, Action{StringBuilder, T})" />
	/// <remarks>
	///     The <paramref name="totalCount" /> is the number of items in total, when the <paramref name="items" /> are
	///     only the first of them.
	/// </remarks>
	public static string Format<T>(IReadOnlyList<T> items, int? totalCount, Action<StringBuilder, T> appendItem)
	{
		int maximum = Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Get();
		StringBuilder stringBuilder = new();
		stringBuilder.Append('[');
		for (int index = 0; index < items.Count; index++)
		{
			stringBuilder.Append(Environment.NewLine).Append("  ");
			if (index >= maximum)
			{
				AppendMoreItems(stringBuilder, (totalCount ?? items.Count) - maximum);
				break;
			}

			appendItem(stringBuilder, items[index]);
			if (index < items.Count - 1)
			{
				stringBuilder.Append(',');
			}
		}

		return stringBuilder.Append(Environment.NewLine).Append(']').ToString();
	}

	/// <summary>
	///     Formats the <paramref name="lines" /> one per line, but at most
	///     <see cref="AwexpectCustomization.FormattingCustomization.MaximumNumberOfCollectionItems" /> of them, followed by
	///     the number of lines that are not listed.
	/// </summary>
	public static string FormatLines(IReadOnlyList<string> lines)
	{
		int maximum = Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Get();
		StringBuilder stringBuilder = new();
		for (int index = 0; index < lines.Count; index++)
		{
			if (index > 0)
			{
				stringBuilder.Append(Environment.NewLine);
			}

			if (index >= maximum)
			{
				AppendMoreItems(stringBuilder, lines.Count - maximum);
				break;
			}

			stringBuilder.Append(lines[index]);
		}

		return stringBuilder.ToString();
	}

	private static void AppendMoreItems(StringBuilder stringBuilder, int count)
		=> stringBuilder.Append("(… and ").Append(count).Append(" more)");
}
