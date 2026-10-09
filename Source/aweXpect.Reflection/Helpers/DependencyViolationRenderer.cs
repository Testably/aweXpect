using System;
using System.Collections.Generic;
using System.Text;

namespace aweXpect.Reflection.Helpers;

/// <summary>
///     Renders the items of the depend-only-on and has-dependencies-outside constraints: one indented line per item,
///     each followed by its list of dependencies outside the allowed set.
/// </summary>
/// <remarks>
///     Shared between the assembly-level and the type-level constraints, so that the formatting (indentation,
///     comma placement, null handling) cannot drift between the two.
/// </remarks>
internal static class DependencyViolationRenderer
{
	/// <summary>
	///     Formats the <paramref name="items" /> as a list with one line per item (appending
	///     <c> depends on […]</c> when <paramref name="violations" /> has an entry for it).
	/// </summary>
	/// <remarks>
	///     A <see langword="null" /> item has no violations to list; it fails because it cannot satisfy the
	///     expectation, so it is rendered without a (contradictory empty) violation list.
	/// </remarks>
	public static string FormatItemsWithDisallowedDependencies<TItem, TViolations>(
		IReadOnlyList<TItem?> items,
		IReadOnlyDictionary<TItem, TViolations> violations)
		where TItem : class
		where TViolations : IEnumerable<string?>
	{
		StringBuilder stringBuilder = new();
		stringBuilder.Append('[');
		for (int index = 0; index < items.Count; index++)
		{
			TItem? item = items[index];
			stringBuilder.Append(Environment.NewLine).Append("  ")
				.Append(Formatter.Format(item));

			if (item is not null && violations.TryGetValue(item, out TViolations? value))
			{
				stringBuilder.Append(" depends on ").Append(Formatter.Format(value));
			}

			if (index < items.Count - 1)
			{
				stringBuilder.Append(',');
			}
		}

		return stringBuilder.Append(Environment.NewLine).Append(']').ToString();
	}
}
