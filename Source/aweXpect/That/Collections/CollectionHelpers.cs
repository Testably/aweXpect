using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Customization;
using aweXpect.Helpers;

namespace aweXpect;

internal static class CollectionHelpers
{
	internal static string CreateDuplicateFailureMessage<TItem>(string it, List<TItem> duplicates)
	{
		StringBuilder sb = new();
		sb.Append(it).Append(" contained ");
		if (duplicates.Count == 1)
		{
			sb.Append("1 duplicate:");
		}
		else
		{
			sb.Append(duplicates.Count).Append(" duplicates:");
		}

		foreach (TItem duplicate in duplicates)
		{
			sb.AppendLine();
			sb.Append("  ");
			Formatter.Format(sb, duplicate);
			sb.Append(',');
		}

		sb.Length--;
		string failure = sb.ToString();
		return failure;
	}

	internal static string GetItemString(this EnumerableQuantifier quantifier)
		=> quantifier.IsSingle() ? "item" : "items";

	internal static ExpectationBuilder AddCollectionContext<TItem>(this ExpectationBuilder expectationBuilder,
		IEnumerable<TItem>? value, bool isIncomplete = false)
	{
		if (value is null)
		{
			return expectationBuilder;
		}

		return expectationBuilder.UpdateContexts(contexts
			=>
		{
			if (contexts.All(c => c.Title != "Collection"))
			{
				contexts
					.Add(new ResultContext.SyncCallback("Collection",
						() => Formatter.Format(value, typeof(TItem).GetFormattingOption(value switch
						{
							ICollection<TItem> coll => coll.Count,
							ICountable countable => countable.Count,
							_ => null,
						})).AppendIsIncomplete(isIncomplete),
						-1));
			}
		});
	}

	internal static ExpectationBuilder AddCollectionContext(this ExpectationBuilder expectationBuilder,
		IEnumerable? value, bool isIncomplete = false)
	{
		if (value is null)
		{
			return expectationBuilder;
		}

		Type type = typeof(object);
		foreach (object? item in value)
		{
			if (item is not null)
			{
				type = item.GetType();
				break;
			}
		}

		return expectationBuilder.UpdateContexts(contexts
			=>
		{
			if (contexts.All(c => c.Title != "Collection"))
			{
				contexts
					.Add(new ResultContext.SyncCallback("Collection",
						() => Formatter.Format(value, type.GetFormattingOption(value switch
						{
							ICollection coll => coll.Count,
							ICountable countable => countable.Count,
							_ => null,
						})).AppendIsIncomplete(isIncomplete),
						-1));
			}
		});
	}

#if NET8_0_OR_GREATER
	internal static async Task<ExpectationBuilder> AddCollectionContext<TItem>(
		this ExpectationBuilder expectationBuilder,
		IMaterializedEnumerable<TItem>? value, bool isIncomplete = false)
	{
		if (value is null)
		{
			return expectationBuilder;
		}

		await value.MaterializeItems(Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Get());

		return expectationBuilder.UpdateContexts(contexts
			=>
		{
			if (contexts.All(c => c.Title != "Collection"))
			{
				contexts
					.Add(new ResultContext.SyncCallback("Collection",
						() => Formatter.Format(value.MaterializedItems,
								typeof(TItem).GetFormattingOption(value.Count))
							.AppendIsIncomplete(isIncomplete),
						-1));
			}
		});
	}
#endif

	internal static ExpectationBuilder AddCollectionContext<TKey, TValue>(this ExpectationBuilder expectationBuilder,
		IDictionary<TKey, TValue>? value, bool isIncomplete = false)
	{
		if (value is null)
		{
			return expectationBuilder;
		}

		return expectationBuilder.UpdateContexts(contexts
			=>
		{
			if (contexts.All(c => c.Title != "Dictionary"))
			{
				contexts
					.Add(new ResultContext.SyncCallback("Dictionary",
						() => Formatter.Format(value, typeof(TValue).GetFormattingOption(value.Count))
							.AppendIsIncomplete(isIncomplete),
						-2));
			}
		});
	}

	internal static ExpectationBuilder AddCollectionContext<TKey, TValue>(this ExpectationBuilder expectationBuilder,
		IReadOnlyDictionary<TKey, TValue>? value, bool isIncomplete = false)
	{
		if (value is null)
		{
			return expectationBuilder;
		}

		return expectationBuilder.UpdateContexts(contexts
			=>
		{
			if (contexts.All(c => c.Title != "Dictionary"))
			{
				contexts
					.Add(new ResultContext.SyncCallback("Dictionary",
						() => Formatter.Format(value, typeof(TValue).GetFormattingOption(value.Count))
							.AppendIsIncomplete(isIncomplete),
						-2));
			}
		});
	}

	internal static bool ExceedsFormatterLimit<TItem>(this IEnumerable<TItem> source)
	{
		int limit = Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Get();
		if (source is ICollection<TItem> collection)
		{
			return collection.Count > limit;
		}

		if (source is ICountable { Count: { } countableCount })
		{
			return countableCount > limit;
		}

		return source.Skip(limit).Any();
	}

	internal static bool ExceedsFormatterLimit(this IEnumerable source)
	{
		int limit = Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Get();
		if (source is ICollection collection)
		{
			return collection.Count > limit;
		}

		if (source is ICountable { Count: { } countableCount })
		{
			return countableCount > limit;
		}

		return source.Cast<object?>().Skip(limit).Any();
	}

	internal static string AppendIsIncomplete(this string formattedItems, bool isIncomplete)
	{
		if (!isIncomplete || formattedItems.Length < 3)
		{
			return formattedItems;
		}

		if (formattedItems.EndsWith("…]"))
		{
			return $"{formattedItems[..^2]}(… and maybe others)]";
		}

		if (formattedItems.EndsWith($"…{Environment.NewLine}]"))
		{
			return formattedItems[..^(Environment.NewLine.Length + 2)] +
			       $"(… and maybe others){Environment.NewLine}]";
		}

		if (formattedItems.EndsWith($"{Environment.NewLine}]"))
		{
			return formattedItems[..^(Environment.NewLine.Length + 1)] +
			       $",{Environment.NewLine}  (… and maybe others){Environment.NewLine}]";
		}

		return $"""
		        {formattedItems[..^1]}, (… and maybe others)]
		        """;
	}

	internal static FormattingOptions GetFormattingOption(this Type type, int? count)
	{
		Type[] singleLineTypes =
		[
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
		if (count < 10 && singleLineTypes.Contains(type))
		{
			return FormattingOptions.SingleLine;
		}

		Type? underlyingType = Nullable.GetUnderlyingType(type);

		if (count < 10 && underlyingType != null &&
		    singleLineTypes.Contains(underlyingType))
		{
			return FormattingOptions.SingleLine;
		}

		return FormattingOptions.MultipleLines;
	}
}
