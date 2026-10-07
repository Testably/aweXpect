using System;
using System.Runtime.CompilerServices;
using aweXpect.Core;
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
	private readonly CollectionIndexOptions _collectionIndexOptions;
	private readonly ExpectationBuilder _expectationBuilder;
	private readonly PredicateOptions<TItem> _options;
	private readonly IThat<TCollection?> _subject;

	internal HasItemWithConditionResult(ExpectationBuilder expectationBuilder,
		IThat<TCollection?> subject,
		CollectionIndexOptions collectionIndexOptions,
		PredicateOptions<TItem> options)
	{
		_expectationBuilder = expectationBuilder;
		_subject = subject;
		_collectionIndexOptions = collectionIndexOptions;
		_options = options;
	}

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	PredicateOptions<TItem> IOptionsProvider<PredicateOptions<TItem>>.Options => _options;

	/// <summary>
	///     …that satisfies the <paramref name="predicate" />.
	/// </summary>
	/// <exception cref="InvalidOperationException">A filter is already set.</exception>
	public HasItemResult<TCollection> Matching(Func<TItem, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
	{
		predicate.ThrowIfNull();
		_options.SetPredicate(predicate,
			$"matching {doNotPopulateThisValue}");
		return new HasItemResult<TCollection>(_expectationBuilder, _subject, _collectionIndexOptions);
	}

	/// <summary>
	///     …of type <typeparamref name="T" />.
	/// </summary>
	/// <exception cref="InvalidOperationException">A filter is already set.</exception>
	public HasItemResult<TCollection> Matching<T>()
	{
		_options.SetPredicate(item => item is T,
			$"of type {Formatter.Format(typeof(T))}");
		return new HasItemResult<TCollection>(_expectationBuilder, _subject, _collectionIndexOptions);
	}

	/// <summary>
	///     …of type <typeparamref name="T" /> that satisfies the <paramref name="predicate" />.
	/// </summary>
	/// <exception cref="InvalidOperationException">A filter is already set.</exception>
	public HasItemResult<TCollection> Matching<T>(Func<T, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
	{
		predicate.ThrowIfNull();
		_options.SetPredicate(item => item is T typed && predicate(typed),
			$"of type {Formatter.Format(typeof(T))} matching {doNotPopulateThisValue}");
		return new HasItemResult<TCollection>(_expectationBuilder, _subject, _collectionIndexOptions);
	}

	/// <summary>
	///     …exactly of type <typeparamref name="T" />.
	/// </summary>
	/// <exception cref="InvalidOperationException">A filter is already set.</exception>
	public HasItemResult<TCollection> MatchingExactly<T>()
	{
		Type exactType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
		_options.SetPredicate(item => item is T && item.GetType() == exactType,
			$"exactly of type {Formatter.Format(typeof(T))}");

		return new HasItemResult<TCollection>(_expectationBuilder, _subject, _collectionIndexOptions);
	}

	/// <summary>
	///     …exactly of type <typeparamref name="T" /> that satisfies the <paramref name="predicate" />.
	/// </summary>
	/// <exception cref="InvalidOperationException">A filter is already set.</exception>
	public HasItemResult<TCollection> MatchingExactly<T>(Func<T, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
	{
		predicate.ThrowIfNull();
		Type exactType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
		_options.SetPredicate(item => item is T typed && item.GetType() == exactType && predicate(typed),
			$"exactly of type {Formatter.Format(typeof(T))} matching {doNotPopulateThisValue}");
		return new HasItemResult<TCollection>(_expectationBuilder, _subject, _collectionIndexOptions);
	}
}
