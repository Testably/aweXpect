using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Customization;
using aweXpect.Helpers;
using aweXpect.Options;
#if NET8_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
#endif

namespace aweXpect;

internal static class CollectionHelpers
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

	private static readonly ConcurrentDictionary<Type, PropertyInfo?> GenericCountProperties = new();

	/// <summary>
	///     Continues the expectation on the collection that the <paramref name="memberAccessor" /> selects from the
	///     subject, rendered as <c>has {memberName} that …</c>.
	/// </summary>
	internal static IThat<IEnumerable<TItem>?> ForCollectionMember<TSource, TItem>(
		this IThat<TSource> subject,
		Func<TSource, IEnumerable<TItem>?> memberAccessor,
		string memberName)
		=> new ThatSubject<IEnumerable<TItem>?>(subject.Get().ExpectationBuilder
			.AddConstraint(memberName,
				static (name, it, grammars) => new HasCollectionMemberConstraint<TSource>(it, grammars, name))
			.ForWhich(memberAccessor, " that ", "it",
				grammars => grammars | ExpectationGrammars.Nested | ExpectationGrammars.Plural, true,
				memberName));

	internal static string GetItemString(this EnumerableQuantifier quantifier)
		=> quantifier.IsSingle() ? "item" : "items";

	/// <summary>
	///     Adds the "Expected" context, listing the <paramref name="expectedItems" /> materialized from the
	///     <paramref name="expected" /> collection.
	/// </summary>
	internal static void AddExpectedItemsContext<TItem>(this ResultContextCollector contexts,
		IEnumerable<TItem> expected, ICollection<TItem> expectedItems)
		=> contexts.Add(new ResultContext.SyncCallback("Expected",
			() => Formatter.Format(expectedItems, typeof(TItem).GetFormattingOption(expected switch
			{
				ICollection<TItem> coll => coll.Count,
				ICountable countable => countable.Count,
				_ => null,
			})),
			-2));

	/// <remarks>
	///     Only the first items are listed, so an endless source of <see langword="null" /> items must not be searched
	///     to its end. An exception of the source is ignored here, as the formatter enumerates the same items and
	///     renders it.
	/// </remarks>
	private static Type GetItemTypeOfListedItems(IEnumerable value)
	{
		IEnumerable<object?> items = value is ICollection
			? value.Cast<object?>()
			: value.Cast<object?>().Take(Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Get());
		try
		{
			return items.GetItemType();
		}
		catch (Exception)
		{
			return typeof(object);
		}
	}

	/// <summary>
	///     Whether the <paramref name="cancellationToken" /> is canceled before all items of the
	///     <paramref name="materialized" /> collection are available.
	/// </summary>
	/// <remarks>
	///     A cancellation only stops reading the items that still have to come from the source, so that a collection
	///     which is already complete in memory is judged like any other value.
	/// </remarks>
	internal static bool IsCanceledBeforeTheEndOf<TItem>(this CancellationToken cancellationToken,
		IEnumerable<TItem> materialized)
		=> cancellationToken.IsCancellationRequested &&
		   materialized is not (ICollection<TItem> or ICountable { Count: not null, });

	/// <inheritdoc cref="IsCanceledBeforeTheEndOf{TItem}(CancellationToken, IEnumerable{TItem})" />
	internal static bool IsCanceledBeforeTheEndOf(this CancellationToken cancellationToken,
		IEnumerable materialized)
		=> cancellationToken.IsCancellationRequested &&
		   materialized is not (ICollection or ICountable { Count: not null, });

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="IsCanceledBeforeTheEndOf{TItem}(CancellationToken, IEnumerable{TItem})" />
	internal static bool IsCanceledBeforeTheEndOf<TItem>(this CancellationToken cancellationToken,
		IAsyncEnumerable<TItem> materialized)
		=> cancellationToken.IsCancellationRequested && materialized is not ICountable { Count: not null, };
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
			if (cancellationToken.IsCanceledBeforeTheEndOf(source))
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
	internal static string? FormatCollection<TItem>(IEnumerable<TItem> value, int? totalCount)
	{
		if (value is IKeyedCollection keyed)
		{
			return keyed.Format();
		}

		if (totalCount is null && value is IMaterializedEnumerable<TItem> { Count: null, } materialized)
		{
			return FormatReadItems(materialized.MaterializedItems, typeof(TItem));
		}

		totalCount ??= value switch
		{
			ICollection<TItem> coll => UserCode.Invoke(static subject => subject.Count, coll),
			ICountable countable => countable.Count,
			_ => null,
		};
		return Formatter.Format(value, typeof(TItem).GetFormattingOption(
			value is LimitedCollection<TItem> limited ? limited.Count : totalCount, totalCount));
	}

	/// <summary>
	///     Formats the untyped <paramref name="value" />, laid out by the type of its listed items.
	/// </summary>
	internal static string? FormatUntypedCollection(IEnumerable value)
	{
		if (value is IMaterializedEnumerable { Count: null, } materialized)
		{
			return FormatReadItems(materialized.MaterializedItems, materialized.MaterializedItems.GetItemType());
		}

		int? totalCount = value switch
		{
			ICollection coll => UserCode.Invoke(static subject => subject.Count, coll),
			ICountable countable => countable.Count,
			_ => value.GetUntypedCount(),
		};
		return Formatter.Format(value, GetItemTypeOfListedItems(value).GetFormattingOption(totalCount, totalCount));
	}

	/// <summary>
	///     The number of items of the untyped <paramref name="value" />, when it is a collection that knows it without
	///     being enumerated.
	/// </summary>
	/// <remarks>
	///     A generic collection that is no <see cref="ICollection" />, e.g. a <see cref="HashSet{T}" />, converts to
	///     <see cref="IReadOnlyCollection{T}" /> of <see langword="object" /> by variance only for reference type items, so
	///     for value type items its count is read by reflection, which is only attempted while the
	///     <see cref="ReflectionFallback" /> is supported.
	/// </remarks>
	internal static int? GetUntypedCount(this IEnumerable? value)
		=> value switch
		{
			null => null,
			ICollection collection => UserCode.Invoke(static subject => subject.Count, collection),
			IReadOnlyCollection<object?> collection => UserCode.Invoke(static subject => subject.Count, collection),
			_ => ReflectionFallback.IsSupported ? ReadGenericCount(value) : null,
		};

#if NET8_0_OR_GREATER
	[RequiresUnreferencedCode("Reads the count of a generic collection interface, which the trimmer may remove.")]
#endif
	private static int? ReadGenericCount(IEnumerable value)
	{
		PropertyInfo? count = GenericCountProperties.GetOrAdd(value.GetType(), static type => type.GetInterfaces()
			.FirstOrDefault(interfaceType => interfaceType.IsGenericType &&
			                                 interfaceType.GetGenericTypeDefinition() is var definition &&
			                                 (definition == typeof(ICollection<>) ||
			                                  definition == typeof(IReadOnlyCollection<>)))
			?.GetProperty(nameof(ICollection.Count)));
		return count is null
			? null
			: UserCode.Invoke(static values => ReadCount(values.Count, values.Subject), (Count: count, Subject: value));
	}

	/// <remarks>
	///     The reflection wraps an exception of the count, which is unwrapped, so that the exception of the collection
	///     fails the expectation.
	/// </remarks>
	private static int? ReadCount(PropertyInfo count, IEnumerable subject)
	{
		try
		{
			return (int?)count.GetValue(subject);
		}
		catch (TargetInvocationException exception) when (exception.InnerException is not null)
		{
			ExceptionDispatchInfo.Capture(exception.InnerException).Throw();
			throw;
		}
	}

	/// <summary>
	///     Formats the items that were read from a source that did not reach its end, marked as incomplete.
	/// </summary>
	/// <remarks>
	///     No further items are read for the context, so that a source that blocks cannot hang the failure message. The
	///     context is left out while nothing is known about the items.
	/// </remarks>
	private static string? FormatReadItems<TItem>(IReadOnlyList<TItem> items, Type itemType)
		=> items.Count == 0
			? null
			: Formatter.Format(HideCount(items), itemType.GetFormattingOption(items.Count)).AppendIsIncomplete(true);

	/// <summary>
	///     The materialized items can be only the first items of the source, so their count must not be rendered as the
	///     number of remaining items.
	/// </summary>
	private static IEnumerable<TItem> HideCount<TItem>(IEnumerable<TItem> items)
	{
		foreach (TItem item in items)
		{
			yield return item;
		}
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
	///     The type that determines the layout of the <paramref name="items" />: for untyped items the type of the first
	///     one that is not <see langword="null" />, like in the "Collection" context.
	/// </summary>
	internal static Type GetItemType(this IEnumerable<object?> items)
		=> items.FirstOrDefault(item => item is not null)?.GetType() ?? typeof(object);

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

#if NET8_0_OR_GREATER
	/// <summary>
	///     Formats the items of the <paramref name="value" /> that were received so far, and marks them as incomplete,
	///     unless the end of the source was reached.
	/// </summary>
	internal static string FormatMaterializedItems<TItem>(this IMaterializedAsyncEnumerable<TItem> value,
		FormattingOptions options)
	{
		int count = value.Count ?? value.MaterializedItems.Count;
		FormattingOptions formattingOptions = typeof(TItem).GetFormattingOption(count, value.Count);
		if (options.UseLineBreaks)
		{
			formattingOptions = formattingOptions with
			{
				UseLineBreaks = true,
			};
		}

		return Formatter.Format(HideCount(value.MaterializedItems), formattingOptions)
			.AppendIsIncomplete(value.Count is null);
	}

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
}
