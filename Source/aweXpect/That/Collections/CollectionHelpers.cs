using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
#if NET8_0_OR_GREATER
using System.Runtime.CompilerServices;
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
	///     subject, rendered as <c>has {memberName} that …</c>.
	/// </summary>
	internal static IThat<IEnumerable<TItem>?> ForCollectionMember<TSource, TItem>(
		this IThat<TSource> subject,
		Func<TSource, IEnumerable<TItem>?> memberAccessor,
		string memberName)
		=> new ThatSubject<IEnumerable<TItem>?>(subject.Get().ExpectationBuilder
			.AddConstraint((it, grammars) => new HasCollectionMemberConstraint<TSource>(it, grammars, memberName))
			.ForWhich(memberAccessor, " that ", "it",
				grammars => grammars | ExpectationGrammars.Plural, negateMemberOnly: true));

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
	///     <c>has lines that …</c>.
	/// </summary>
	/// <remarks>
	///     The parent renders the separator <c>" that "</c> before it knows that a quantifier follows, and
	///     <c>that at least 2 are …</c> is not grammatical, so the separator is replaced with <c>" of which "</c>.
	/// </remarks>
	internal static void AppendNestedQuantifier(this StringBuilder stringBuilder, EnumerableQuantifier quantifier,
		bool isNegated)
	{
		const string that = " that ";
		if (stringBuilder.Length >= that.Length &&
		    stringBuilder.ToString(stringBuilder.Length - that.Length, that.Length) == that)
		{
			stringBuilder.Length -= that.Length;
			stringBuilder.Append(" of which ");
		}

		if (isNegated)
		{
			quantifier.AppendNestedNegated(stringBuilder);
		}
		else
		{
			stringBuilder.Append(quantifier);
		}

		stringBuilder.Append(' ');
	}

	/// <summary>
	///     Adds the <paramref name="matchingItems" /> and the <paramref name="notMatchingItems" /> of the
	///     <paramref name="constraint" /> as context, as far as the <paramref name="quantifier" /> requests them.
	/// </summary>
	/// <remarks>
	///     The negation is applied after the evaluation, so the contexts for both cases are added and each is only
	///     shown while the <paramref name="constraint" /> requests it.
	/// </remarks>
	internal static void AddQuantifierContexts(this ExpectationBuilder expectationBuilder,
		ConstraintResult constraint, EnumerableQuantifier quantifier,
		Func<string>? matchingItems, Func<string>? notMatchingItems)
	{
		EnumerableQuantifier.QuantifierContexts normal = quantifier.GetQuantifierContext();
		EnumerableQuantifier.QuantifierContexts negated = quantifier.GetNegatedQuantifierContext();

		void Add(string title, EnumerableQuantifier.QuantifierContexts context, Func<string>? items)
		{
			if (items is not null && (normal | negated).HasFlag(context))
			{
				expectationBuilder.AddQuantifierItemsContext(title, constraint, () =>
				{
					EnumerableQuantifier.QuantifierContexts shown = constraint.Grammars.IsNegated() ? negated : normal;
					return shown.HasFlag(context) ? items() : null;
				});
			}
		}

		// The context of the expectation that is not negated is added first to keep its position among the contexts.
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

	private static void AddQuantifierItemsContext(this ExpectationBuilder expectationBuilder, string title,
		ConstraintResult owner, Func<string?> content)
		=> expectationBuilder.UpdateContexts(contexts =>
		{
			ResultContext? existing = contexts.FirstOrDefault(context => context.Title == title);
			if (existing is null)
			{
				contexts.Add(new QuantifierItemsContext(title, owner, content));
			}
			else if (existing is QuantifierItemsContext itemsContext)
			{
				itemsContext.Add(owner, content);
			}
		});

	/// <summary>
	///     The matching or not matching items of the quantified constraints that share an expectation builder.
	/// </summary>
	/// <remarks>
	///     The first constraint whose items are shown provides the content. A constraint that is evaluated again keeps
	///     the items of its first evaluation.
	/// </remarks>
	private sealed class QuantifierItemsContext : ResultContext
	{
		private readonly List<(ConstraintResult Owner, Func<string?> Content)> _candidates = [];

		public QuantifierItemsContext(string title, ConstraintResult owner, Func<string?> content)
			: base(title, int.MaxValue)
			=> Add(owner, content);

		public void Add(ConstraintResult owner, Func<string?> content)
		{
			if (_candidates.All(candidate => !ReferenceEquals(candidate.Owner, owner)))
			{
				_candidates.Add((owner, content));
			}
		}

		public override Task<string?> GetContent(CancellationToken cancellationToken = default)
			=> Task.FromResult(_candidates
				.Select(candidate => candidate.Content())
				.FirstOrDefault(content => content is not null));
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

		// Only the first items are listed, so an endless source of null items must not be searched to its end.
		IEnumerable<object?> items = value is ICollection
			? value.Cast<object?>()
			: value.Cast<object?>().Take(Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Get());
		Type type = items.FirstOrDefault(item => item is not null)?.GetType() ?? typeof(object);

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
	///     Counts the items of the <paramref name="source" />, or returns <see langword="null" /> when the
	///     <paramref name="cancellationToken" /> is canceled before the <paramref name="source" /> ends.
	/// </summary>
	internal static int? CountUnlessCanceled<TItem>(this IEnumerable<TItem> source,
		CancellationToken cancellationToken)
	{
		int count = 0;
		foreach (TItem _ in source)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return null;
			}

			count++;
		}

		return count;
	}

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
		if (count < 10 && (type.IsEnum || singleLineTypes.Contains(type)))
		{
			return FormattingOptions.SingleLine with
			{
				TotalItemCount = totalCount,
			};
		}

		Type? underlyingType = Nullable.GetUnderlyingType(type);

		if (count < 10 && underlyingType != null &&
		    (underlyingType.IsEnum || singleLineTypes.Contains(underlyingType)))
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
