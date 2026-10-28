using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Extending;
using aweXpect.Options;

namespace aweXpect.Reflection.Helpers;

/// <summary>
///     Adds an <see cref="ItemExpectation{TItem}" /> on the items of a collection, for every kind of subject of the
///     plural expectations: the collection itself, the items a quantifier selects, also of a struct collection, and
///     their asynchronous counterparts.
/// </summary>
/// <remarks>
///     A <see langword="null" /> item cannot be inspected, so it fails the expectation and its negation without
///     reaching <see cref="ItemExpectation{TItem}.IsMatch" />.
/// </remarks>
internal static class ItemExpectationConstraints
{
	/// <summary>
	///     Expects all items of the <paramref name="subject" /> to meet the <paramref name="expectation" />.
	/// </summary>
	public static ExpectationBuilder ExpectAll<TItem>(this IThat<IEnumerable<TItem?>?> subject,
		ItemExpectation<TItem> expectation)
		where TItem : class
		=> subject.Get().ExpectationBuilder.AddConstraint(expectation,
			static (expectation, it, grammars) => new ItemsConstraint<IEnumerable<TItem?>?, TItem>(it, grammars,
				EnumerableQuantifier.All(), true, expectation));

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="ExpectAll{TItem}(IThat{IEnumerable{TItem}}, ItemExpectation{TItem})" />
	public static ExpectationBuilder ExpectAll<TItem>(this IThat<IAsyncEnumerable<TItem?>?> subject,
		ItemExpectation<TItem> expectation)
		where TItem : class
		=> subject.Get().ExpectationBuilder.AddConstraint(expectation,
			static (expectation, it, grammars) => new AsyncItemsConstraint<TItem>(it, grammars,
				EnumerableQuantifier.All(), true, expectation));
#endif

	/// <summary>
	///     Expects no item of the <paramref name="subject" /> to meet the <paramref name="expectation" />.
	/// </summary>
	public static ExpectationBuilder ExpectNone<TItem>(this IThat<IEnumerable<TItem?>?> subject,
		ItemExpectation<TItem> expectation)
		where TItem : class
		=> subject.Get().ExpectationBuilder.AddConstraint(expectation,
			static (expectation, it, grammars) => new ItemsConstraint<IEnumerable<TItem?>?, TItem>(it, grammars,
				EnumerableQuantifier.None(ExpectationGrammars.Nested), true, expectation));

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="ExpectNone{TItem}(IThat{IEnumerable{TItem}}, ItemExpectation{TItem})" />
	public static ExpectationBuilder ExpectNone<TItem>(this IThat<IAsyncEnumerable<TItem?>?> subject,
		ItemExpectation<TItem> expectation)
		where TItem : class
		=> subject.Get().ExpectationBuilder.AddConstraint(expectation,
			static (expectation, it, grammars) => new AsyncItemsConstraint<TItem>(it, grammars,
				EnumerableQuantifier.None(ExpectationGrammars.Nested), true, expectation));
#endif

	/// <summary>
	///     Expects the items that the quantifier of the <paramref name="items" /> selects to meet the
	///     <paramref name="expectation" />.
	/// </summary>
	public static ExpectationBuilder ExpectItems<TItem>(this IEnumerableItems<TItem?> items,
		ItemExpectation<TItem> expectation)
		where TItem : class
		=> items.Subject.Get().ExpectationBuilder.AddConstraint((items.Quantifier, expectation),
			static (state, it, grammars) => new ItemsConstraint<IEnumerable<TItem?>?, TItem>(it, grammars,
				state.Quantifier, false, state.expectation));

	/// <inheritdoc cref="ExpectItems{TItem}(IEnumerableItems{TItem}, ItemExpectation{TItem})" />
	public static ExpectationBuilder ExpectItems<TCollection, TItem>(
		this IStructEnumerableItems<TCollection, TItem?> items,
		ItemExpectation<TItem> expectation)
		where TCollection : struct, IEnumerable<TItem?>
		where TItem : class
		=> items.Subject.Get().ExpectationBuilder.AddConstraint((items.Quantifier, expectation),
			static (state, it, grammars) => new ItemsConstraint<TCollection, TItem>(it, grammars,
				state.Quantifier, false, state.expectation));

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="ExpectItems{TItem}(IEnumerableItems{TItem}, ItemExpectation{TItem})" />
	public static ExpectationBuilder ExpectItems<TItem>(this IAsyncEnumerableItems<TItem?> items,
		ItemExpectation<TItem> expectation)
		where TItem : class
		=> items.Subject.Get().ExpectationBuilder.AddConstraint((items.Quantifier, expectation),
			static (state, it, grammars) => new AsyncItemsConstraint<TItem>(it, grammars,
				state.Quantifier, false, state.expectation));
#endif

	private sealed class ItemsConstraint<TCollection, TItem>(
		string it,
		ExpectationGrammars grammars,
		EnumerableQuantifier quantifier,
		bool isOnCollection,
		ItemExpectation<TItem> expectation)
		: GeneratedCollectionConstraint<TCollection, TItem?>(it, grammars, quantifier, isOnCollection)
		where TCollection : IEnumerable<TItem?>?
		where TItem : class
	{
		protected override string Verb => expectation.Verb;

		protected override ValueTask<PredicateResult> IsMatch(TItem? item, CancellationToken cancellationToken)
			=> item is null
				? new ValueTask<PredicateResult>(ItemResults.Null)
				: expectation.IsMatch(item, cancellationToken);

		protected override void AppendItemExpectation(StringBuilder stringBuilder, ExpectationGrammars grammars,
			string? indentation)
			=> expectation.AppendExpectation(stringBuilder, grammars);

		protected override string FormatItems(IEnumerable<TItem?> items, FormattingOptions options)
			=> expectation.FormatItems(items, options);
	}

#if NET8_0_OR_GREATER
	private sealed class AsyncItemsConstraint<TItem>(
		string it,
		ExpectationGrammars grammars,
		EnumerableQuantifier quantifier,
		bool isOnCollection,
		ItemExpectation<TItem> expectation)
		: GeneratedAsyncCollectionConstraint<TItem?>(it, grammars, quantifier, isOnCollection)
		where TItem : class
	{
		protected override string Verb => expectation.Verb;

		protected override ValueTask<PredicateResult> IsMatch(TItem? item, CancellationToken cancellationToken)
			=> item is null
				? new ValueTask<PredicateResult>(ItemResults.Null)
				: expectation.IsMatch(item, cancellationToken);

		protected override void AppendItemExpectation(StringBuilder stringBuilder, ExpectationGrammars grammars,
			string? indentation)
			=> expectation.AppendExpectation(stringBuilder, grammars);

		protected override string FormatItems(IEnumerable<TItem?> items, FormattingOptions options)
			=> expectation.FormatItems(items, options);
	}
#endif
}
