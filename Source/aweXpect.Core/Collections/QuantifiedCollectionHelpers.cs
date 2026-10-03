using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Options;

namespace aweXpect;

/// <summary>
///     The contexts and the item formatting of the <see cref="QuantifiedCollectionConstraintBase{TValue,TItem}" />.
/// </summary>
/// <remarks>
///     aweXpect keeps its own copies of <see cref="AppendIsIncomplete" /> and <see cref="GetFormattingOption" /> for the
///     other collection expectations, so the "Collection" context is laid out alike.
/// </remarks>
internal static class QuantifiedCollectionHelpers
{
	private const string MaybeMoreMarker = "(… and maybe more)";

	private static readonly Type[] SingleLineTypes =
	[
		typeof(bool),
		typeof(char),
		typeof(byte),
		typeof(sbyte),
		typeof(float),
		typeof(double),
		typeof(decimal),
		typeof(int),
		typeof(uint),
		typeof(long),
		typeof(ulong),
		typeof(short),
		typeof(ushort),
#if NET8_0_OR_GREATER
		typeof(Int128),
		typeof(UInt128),
		typeof(Half),
#endif
	];

	/// <summary>
	///     Adds the matching and the not matching items as context, as far as they are <paramref name="shown" />.
	/// </summary>
	internal static void AddQuantifierContexts(this ResultContextCollector contexts,
		EnumerableQuantifier quantifier, EnumerableQuantifier.QuantifierContexts shown,
		Func<string>? matchingItems, Func<string>? notMatchingItems)
	{
		EnumerableQuantifier.QuantifierContexts normal = quantifier.GetQuantifierContext();

		void Add(string title, EnumerableQuantifier.QuantifierContexts context, Func<string>? items)
		{
			if (items is not null && shown.HasFlag(context))
			{
				contexts.Add(new ResultContext.SyncCallback(title, items, int.MaxValue));
			}
		}

		// The context of the expectation that is not negated comes first, as both have the same priority.
		if (normal.HasFlag(EnumerableQuantifier.QuantifierContexts.NotMatchingItems))
		{
			Add("Not matching items", EnumerableQuantifier.QuantifierContexts.NotMatchingItems, notMatchingItems);
			Add("Matching items", EnumerableQuantifier.QuantifierContexts.MatchingItems, matchingItems);
		}
		else
		{
			Add("Matching items", EnumerableQuantifier.QuantifierContexts.MatchingItems, matchingItems);
			Add("Not matching items", EnumerableQuantifier.QuantifierContexts.NotMatchingItems, notMatchingItems);
		}
	}

	/// <summary>
	///     Formats the <paramref name="items" /> recorded from the <paramref name="source" /> collection, together with
	///     their keys when the source is an <see cref="IKeyedCollection" />.
	/// </summary>
	/// <remarks>
	///     Only the first items are recorded, so <paramref name="totalCount" /> is how many were found in total.
	/// </remarks>
	internal static string Format<TItem>(this LimitedCollection<TItem> items, object? source, Type itemType,
		int? totalCount)
		=> source is IKeyedCollection keyed
			? keyed.Format(items.Indices, totalCount)
			: Formatter.Format(items, itemType.GetFormattingOption(items.Count, totalCount));

	internal static string AppendIsIncomplete(this string formattedItems, bool isIncomplete)
	{
		if (!isIncomplete || formattedItems.Length < 3)
		{
			return formattedItems;
		}

		// The count of a collection whose enumeration stopped early does not tell how many items remain.
		Match truncation = Regex.Match(formattedItems, @"\(… and [^)]+ more\)(?=(\r?\n)?\]$)",
			RegexOptions.None, TimeSpan.FromSeconds(1));
		if (truncation.Success)
		{
			return formattedItems[..truncation.Index] + MaybeMoreMarker +
			       formattedItems[(truncation.Index + truncation.Length)..];
		}

		if (formattedItems.EndsWith($"{Environment.NewLine}]"))
		{
			return formattedItems[..^(Environment.NewLine.Length + 1)] +
			       $",{Environment.NewLine}  {MaybeMoreMarker}{Environment.NewLine}]";
		}

		return $"{formattedItems[..^1]}, {MaybeMoreMarker}]";
	}

	/// <summary>
	///     The layout follows the <paramref name="count" /> of items that are rendered, while a truncation marker names
	///     the remainder of the <paramref name="totalCount" /> items the collection holds.
	/// </summary>
	internal static FormattingOptions GetFormattingOption(this Type type, int? count, int? totalCount = null)
	{
		if (count < 10 && (type.IsEnum || SingleLineTypes.Contains(type)))
		{
			return FormattingOptions.SingleLine with
			{
				TotalItemCount = totalCount,
			};
		}

		Type? underlyingType = Nullable.GetUnderlyingType(type);

		if (count < 10 && underlyingType != null &&
		    (underlyingType.IsEnum || SingleLineTypes.Contains(underlyingType)))
		{
			return FormattingOptions.SingleLine with
			{
				TotalItemCount = totalCount,
			};
		}

		return FormattingOptions.MultipleLines with
		{
			TotalItemCount = totalCount,
		};
	}
}
