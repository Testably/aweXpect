using System;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Core.Extending;
using aweXpect.Helpers;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     The result for verifying that a collection has an item…
/// </summary>
/// <remarks>
///     <seealso cref="ExpectationResult{TType,TSelf}" />
/// </remarks>
public class HasItemWithConditionResult<TCollection, TItem>
	: IOptionsProvider<PredicateOptions<TItem>>
{
	private readonly IOptionsHolderSource<CollectionIndexOptions> _collectionIndexOptions;
	private readonly ExpectationBuilder _expectationBuilder;
	private readonly IOptionsHolderSource<PredicateOptions<TItem>> _options;
	private readonly IThat<TCollection?> _subject;

	internal HasItemWithConditionResult(ExpectationBuilder expectationBuilder,
		IThat<TCollection?> subject,
		IOptionsHolderSource<CollectionIndexOptions> collectionIndexOptions,
		IOptionsHolderSource<PredicateOptions<TItem>> options)
	{
		_expectationBuilder = expectationBuilder;
		_subject = subject;
		_collectionIndexOptions = collectionIndexOptions;
		_options = options;
	}

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	OptionsHolder<PredicateOptions<TItem>> IOptionsProvider<PredicateOptions<TItem>>.Options => _options.Holder;

	/// <summary>
	///     …that satisfies the <paramref name="predicate" />.
	/// </summary>
	public HasItemResult<TCollection, IThat<TCollection?>> Matching(Func<TItem, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
	{
		predicate.ThrowIfNull();
		_options.Holder.Value = new PredicateOptions<TItem>(predicate,
			$"matching {doNotPopulateThisValue.TrimCommonWhiteSpace()}");
		return new HasItemResult<TCollection, IThat<TCollection?>>(_expectationBuilder, _subject, _collectionIndexOptions);
	}

	/// <summary>
	///     …of type <typeparamref name="T" />.
	/// </summary>
	public HasItemResult<TCollection, IThat<TCollection?>> Matching<T>()
	{
		_options.Holder.Value = new PredicateOptions<TItem>(item => item is T,
			$"of type {Formatter.Format(typeof(T))}");
		return new HasItemResult<TCollection, IThat<TCollection?>>(_expectationBuilder, _subject, _collectionIndexOptions);
	}

	/// <summary>
	///     …of type <typeparamref name="T" /> that satisfies the <paramref name="predicate" />.
	/// </summary>
	public HasItemResult<TCollection, IThat<TCollection?>> Matching<T>(Func<T, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
	{
		predicate.ThrowIfNull();
		_options.Holder.Value = new PredicateOptions<TItem>(item => item is T typed && predicate(typed),
			$"of type {Formatter.Format(typeof(T))} matching {doNotPopulateThisValue.TrimCommonWhiteSpace()}");
		return new HasItemResult<TCollection, IThat<TCollection?>>(_expectationBuilder, _subject, _collectionIndexOptions);
	}

	/// <summary>
	///     …exactly of type <typeparamref name="T" />.
	/// </summary>
	public HasItemResult<TCollection, IThat<TCollection?>> MatchingExactly<T>()
	{
		Type exactType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
		_options.Holder.Value = new PredicateOptions<TItem>(item => item is T && item.GetType() == exactType,
			$"exactly of type {Formatter.Format(typeof(T))}");

		return new HasItemResult<TCollection, IThat<TCollection?>>(_expectationBuilder, _subject, _collectionIndexOptions);
	}

	/// <summary>
	///     …exactly of type <typeparamref name="T" /> that satisfies the <paramref name="predicate" />.
	/// </summary>
	public HasItemResult<TCollection, IThat<TCollection?>> MatchingExactly<T>(Func<T, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
	{
		predicate.ThrowIfNull();
		Type exactType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
		_options.Holder.Value = new PredicateOptions<TItem>(item => item is T typed && item.GetType() == exactType && predicate(typed),
			$"exactly of type {Formatter.Format(typeof(T))} matching {doNotPopulateThisValue.TrimCommonWhiteSpace()}");
		return new HasItemResult<TCollection, IThat<TCollection?>>(_expectationBuilder, _subject, _collectionIndexOptions);
	}
}
