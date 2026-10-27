using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Extending;
using aweXpect.Helpers;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     The result for verifying that an asynchronous collection contains a single item.
/// </summary>
/// <remarks>
///     <seealso cref="ExpectationResult{TType,TSelf}" />
/// </remarks>
public class AsyncSingleItemResult<TCollection, TItem>
	: ExpectationResult<TItem, AsyncSingleItemResult<TCollection, TItem>>,
		IOptionsProvider<PredicateOptions<TItem>>
{
	private readonly IOptionsHolderSource<PredicateOptions<TItem>> _options;
	private readonly SingleItemSource<TItem> _source;

	internal AsyncSingleItemResult(ExpectationBuilder expectationBuilder,
		IOptionsHolderSource<PredicateOptions<TItem>> options,
		SingleItemSource<TItem> source)
		: base(expectationBuilder)
	{
		_options = options;
		_source = source;
	}

	/// <summary>
	///     Further expectations on the single item.
	/// </summary>
	public IThatSubject<TItem> Which
		=> new ThatSubject<TItem>(ExpectationBuilder.ForAsyncWhich<TCollection, TItem>(_source.GetAsync, " that ",
			"it", grammars => grammars & ~ExpectationGrammars.Plural));

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	OptionsHolder<PredicateOptions<TItem>> IOptionsProvider<PredicateOptions<TItem>>.Options => _options.Holder;

	/// <summary>
	///     …that satisfies the <paramref name="predicate" />.
	/// </summary>
	public AsyncSingleMatchingItemResult<TCollection, TItem> Matching(Func<TItem, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
	{
		predicate.ThrowIfNull();
		_options.Holder.Value = new PredicateOptions<TItem>(predicate,
			$" matching {doNotPopulateThisValue.TrimCommonWhiteSpace()}");
		return new AsyncSingleMatchingItemResult<TCollection, TItem>(ExpectationBuilder, _source.GetAsync<TCollection>);
	}

	/// <summary>
	///     …of type <typeparamref name="T" />.
	/// </summary>
	public AsyncSingleMatchingItemResult<TCollection, T> Matching<T>()
	{
		_options.Holder.Value = new PredicateOptions<TItem>(item => item is T,
			$" of type {Formatter.Format(typeof(T))}");
		return Cast<T>(x => (T)(object)x!);
	}

	/// <summary>
	///     …of type <typeparamref name="T" /> that satisfies the <paramref name="predicate" />.
	/// </summary>
	public AsyncSingleMatchingItemResult<TCollection, T> Matching<T>(Func<T, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
	{
		predicate.ThrowIfNull();
		_options.Holder.Value = new PredicateOptions<TItem>(item => item is T typed && predicate(typed),
			$" of type {Formatter.Format(typeof(T))} matching {doNotPopulateThisValue.TrimCommonWhiteSpace()}");
		return Cast<T>(x => (T)(object)x!);
	}

	/// <summary>
	///     …exactly of type <typeparamref name="T" />.
	/// </summary>
	public AsyncSingleMatchingItemResult<TCollection, T> MatchingExactly<T>()
	{
		Type exactType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
		_options.Holder.Value = new PredicateOptions<TItem>(item => item is T && item.GetType() == exactType,
			$" exactly of type {Formatter.Format(typeof(T))}");
		return Cast<T>(x => (T)(object)x!);
	}

	/// <summary>
	///     …exactly of type <typeparamref name="T" /> that satisfies the <paramref name="predicate" />.
	/// </summary>
	public AsyncSingleMatchingItemResult<TCollection, T> MatchingExactly<T>(Func<T, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
	{
		predicate.ThrowIfNull();
		Type exactType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
		_options.Holder.Value = new PredicateOptions<TItem>(item => item is T typed && item.GetType() == exactType && predicate(typed),
			$" exactly of type {Formatter.Format(typeof(T))} matching {doNotPopulateThisValue.TrimCommonWhiteSpace()}");
		return Cast<T>(x => (T)(object)x!);
	}

	private AsyncSingleMatchingItemResult<TCollection, T> Cast<T>(Func<TItem?, T> memberAccessor)
		=> new(ExpectationBuilder, async x => memberAccessor(await _source.GetAsync(x)));
}
