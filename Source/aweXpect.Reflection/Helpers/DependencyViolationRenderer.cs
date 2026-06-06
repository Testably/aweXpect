using System;
using System.Collections.Generic;
using System.Text;
using aweXpect.Core;

namespace aweXpect.Reflection.Helpers;

/// <summary>
///     Renders the grouped failure output of the depend-only-on and has-dependencies-outside constraints: one
///     indented line per failing item, each followed by its list of dependencies outside the allowed set.
/// </summary>
/// <remarks>
///     Shared between the assembly-level and the type-level constraints, so that the formatting (indentation,
///     comma placement, null handling) cannot drift between the two.
/// </remarks>
internal static class DependencyViolationRenderer
{
	/// <summary>
	///     Appends <c>{it}{header}[</c>, one indented line per item in <paramref name="items" /> (appending
	///     <c> depends on […]</c> when <paramref name="violations" /> has an entry for it), and a closing <c>]</c>.
	/// </summary>
	/// <remarks>
	///     A <see langword="null" /> item has no violations to list; it fails because it cannot satisfy the
	///     expectation, so it is rendered without a (contradictory empty) violation list.
	/// </remarks>
	public static void AppendItemsWithDisallowedDependencies<TItem, TViolations>(
		StringBuilder stringBuilder,
		string it,
		string header,
		IReadOnlyList<TItem?> items,
		IReadOnlyDictionary<TItem, TViolations> violations,
		string? indentation)
		where TItem : class
		where TViolations : IEnumerable<string?>
	{
		string itemIndentation = (indentation ?? string.Empty) + "  ";
		stringBuilder.Append(it).Append(header).Append('[');
		for (int index = 0; index < items.Count; index++)
		{
			TItem? item = items[index];
			stringBuilder.Append(Environment.NewLine).Append(itemIndentation)
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

		stringBuilder.Append(Environment.NewLine).Append(indentation).Append(']');
	}
}
