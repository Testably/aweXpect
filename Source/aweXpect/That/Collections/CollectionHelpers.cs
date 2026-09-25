using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
#if NET8_0_OR_GREATER
using System.Runtime.CompilerServices;
using System.Threading;
#endif
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Customization;
using aweXpect.Helpers;
using aweXpect.Options;

namespace aweXpect;

internal static class CollectionHelpers
{
	private const string MaybeMoreMarker = "(… and maybe more)";

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

	/// <summary>
	///     Continues the expectation on the collection that the <paramref name="memberAccessor" /> selects from the
	///     subject, rendered as <c>has {memberName} which …</c>.
	/// </summary>
	internal static IThat<IEnumerable<TItem>?> ForCollectionMember<TSource, TItem>(
		this IThat<TSource> subject,
		Func<TSource, IEnumerable<TItem>?> memberAccessor,
		string memberName)
		=> new ThatSubject<IEnumerable<TItem>?>(subject.Get().ExpectationBuilder
			.AddConstraint((it, grammars) => new HasCollectionMemberConstraint<TSource>(it, grammars, memberName))
			.ForWhich(memberAccessor, " which ",
				expectationGrammar: grammars => grammars | ExpectationGrammars.Plural, negateMemberOnly: true));

	/// <summary>
	///     Names the member in the expectation text and rules a <see langword="null" /> subject out. A negation applies
	///     to the continued expectation, so the text is the same in both grammars.
	/// </summary>
	private sealed class HasCollectionMemberConstraint<TSource>(
		string it,
		ExpectationGrammars grammars,
		string memberName)
		: ConstraintResult.WithNotNullValue<TSource>(it, grammars),
			IValueConstraint<TSource>
	{
		public ConstraintResult IsMetBy(TSource actual)
		{
			Actual = actual;
			Outcome = actual is null ? Outcome.Failure : Outcome.Success;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("has ").Append(memberName);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalExpectation(stringBuilder, indentation);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
		}
	}

	internal static string GetItemString(this EnumerableQuantifier quantifier)
		=> quantifier.IsSingle() ? "item" : "items";

	/// <summary>
	///     Appends the <paramref name="quantifier" /> of a nested collection expectation, e.g. in
	///     <c>has lines which …</c>.
	/// </summary>
	/// <remarks>
	///     The parent renders the separator <c>" which "</c> before it knows that a quantifier follows, and
	///     <c>which at least 2 are …</c> is not grammatical, so the separator is completed to <c>" of which "</c>.
	/// </remarks>
	internal static void AppendNestedQuantifier(this StringBuilder stringBuilder, EnumerableQuantifier quantifier,
		bool isNegated)
	{
		const string which = " which ";
		if (stringBuilder.Length >= which.Length &&
		    stringBuilder.ToString(stringBuilder.Length - which.Length, which.Length) == which)
		{
			stringBuilder.Insert(stringBuilder.Length - which.Length + 1, "of ");
		}

		if (isNegated)
		{
			stringBuilder.Append("not ");
		}

		stringBuilder.Append(quantifier).Append(' ');
	}

	/// <summary>
	///     Adds the "Collection" context for the <paramref name="value" />, passing the <paramref name="totalCount" />
	///     of items whenever the caller counted them while the <paramref name="value" /> kept only the first ones.
	/// </summary>
	internal static ExpectationBuilder AddCollectionContext<TItem>(this ExpectationBuilder expectationBuilder,
		IEnumerable<TItem>? value, bool isIncomplete = false, int? totalCount = null)
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
						() => FormatCollection(value, totalCount).AppendIsIncomplete(isIncomplete),
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
						() => FormatCollection(value, type).AppendIsIncomplete(isIncomplete),
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
						() => Formatter.Format(HideCount(value.MaterializedItems),
								typeof(TItem).GetFormattingOption(value.Count ?? value.MaterializedItems.Count,
									value.Count))
							.AppendIsIncomplete(isIncomplete || value.Count is null),
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

#if NET8_0_OR_GREATER
	/// <summary>
	///     Enumerates the <paramref name="source" /> until it ends or the <paramref name="cancellationToken" /> is
	///     canceled, also while it waits for the next item.
	/// </summary>
	/// <remarks>
	///     For expectations that report a canceled evaluation as undecided, which must not be aborted instead, when the
	///     <paramref name="source" /> throws because of the cancellation instead of providing the next item.
	/// </remarks>
	internal static async IAsyncEnumerable<TItem> UntilCancelled<TItem>(this IAsyncEnumerable<TItem> source,
		[EnumeratorCancellation] CancellationToken cancellationToken)
	{
		await using IAsyncEnumerator<TItem> enumerator = source.GetAsyncEnumerator(cancellationToken);
		while (await MoveNextUntilCancelled(enumerator, cancellationToken))
		{
			yield return enumerator.Current;
		}
	}

	private static async ValueTask<bool> MoveNextUntilCancelled<TItem>(IAsyncEnumerator<TItem> enumerator,
		CancellationToken cancellationToken)
	{
		try
		{
			return await enumerator.MoveNextAsync();
		}
		catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
		{
			return false;
		}
	}
#endif

	/// <summary>
	///     A <see cref="LimitedCollection{T}" /> keeps only the first items, so its count drives the layout but must not
	///     be rendered as the total from which the number of remaining items is derived.
	/// </summary>
	private static string FormatCollection<TItem>(IEnumerable<TItem> value, int? totalCount)
	{
		if (value is IKeyedCollection keyed)
		{
			return keyed.Format();
		}

		totalCount ??= value switch
		{
			ICollection<TItem> coll => coll.Count,
			ICountable countable => countable.Count,
			_ => null,
		};
		return Formatter.Format(value, typeof(TItem).GetFormattingOption(
			value is LimitedCollection<TItem> limited ? limited.Count : totalCount, totalCount));
	}

	private static string FormatCollection(IEnumerable value, Type itemType)
	{
		int? totalCount = value switch
		{
			ICollection coll => coll.Count,
			ICountable countable => countable.Count,
			_ => null,
		};
		return Formatter.Format(value, itemType.GetFormattingOption(totalCount, totalCount));
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

#if NET8_0_OR_GREATER
	/// <summary>
	///     The materialized items can be only the first items of the asynchronous enumerable, so their count must not be
	///     rendered as the number of remaining items.
	/// </summary>
	private static IEnumerable<TItem> HideCount<TItem>(IEnumerable<TItem> items)
	{
		foreach (TItem item in items)
		{
			yield return item;
		}
	}
#endif

	internal static bool ExceedsFormatterLimit<TItem>(this IEnumerable<TItem> subject)
	{
		int limit = Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Get();
		if (subject is ICollection<TItem> collection)
		{
			return collection.Count > limit;
		}

		if (subject is ICountable { Count: { } countableCount })
		{
			return countableCount > limit;
		}

		return subject.Skip(limit).Any();
	}

	internal static bool ExceedsFormatterLimit(this IEnumerable subject)
	{
		int limit = Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Get();
		if (subject is ICollection collection)
		{
			return collection.Count > limit;
		}

		if (subject is ICountable { Count: { } countableCount })
		{
			return countableCount > limit;
		}

		return subject.Cast<object?>().Skip(limit).Any();
	}

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
			return FormattingOptions.SingleLine with
			{
				TotalItemCount = totalCount,
			};
		}

		Type? underlyingType = Nullable.GetUnderlyingType(type);

		if (count < 10 && underlyingType != null &&
		    singleLineTypes.Contains(underlyingType))
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
