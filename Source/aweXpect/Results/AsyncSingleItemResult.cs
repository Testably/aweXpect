using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using aweXpect.Core;
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
	private readonly Func<TCollection, Task<TItem?>> _asyncMemberAccessor;
	private readonly ExpectationBuilder _expectationBuilder;
	private readonly PredicateOptions<TItem> _options;

	internal AsyncSingleItemResult(ExpectationBuilder expectationBuilder,
		PredicateOptions<TItem> options,
		Func<TCollection, Task<TItem?>> asyncMemberAccessor)
		: base(expectationBuilder)
	{
		_expectationBuilder = expectationBuilder;
		_options = options;
		_asyncMemberAccessor = asyncMemberAccessor;
	}

	/// <summary>
	///     Further expectations on the single item.
	/// </summary>
	public IThat<TItem> Which
		=> new ThatSubject<TItem>(_expectationBuilder.ForWhich(_asyncMemberAccessor, " that "));

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	PredicateOptions<TItem> IOptionsProvider<PredicateOptions<TItem>>.Options => _options;

	/// <summary>
	///     …that satisfies the <paramref name="predicate" />.
	/// </summary>
	public AsyncSingleItemResult<TCollection, TItem> Matching(Func<TItem, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
	{
		predicate.ThrowIfNull();
		_options.SetPredicate(predicate,
			$" matching {doNotPopulateThisValue}");
		return this;
	}

	/// <summary>
	///     …of type <typeparamref name="T" />.
	/// </summary>
	public AsyncSingleItemResult<TCollection, T> Matching<T>()
	{
		_options.SetPredicate(item => item is T,
			$" of type {Formatter.Format(typeof(T))}");
		return Cast<T>(x => (T)(object)x!);
	}

	/// <summary>
	///     …of type <typeparamref name="T" /> that satisfies the <paramref name="predicate" />.
	/// </summary>
	public AsyncSingleItemResult<TCollection, T> Matching<T>(Func<T, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
	{
		predicate.ThrowIfNull();
		_options.SetPredicate(item => item is T typed && predicate(typed),
			$" of type {Formatter.Format(typeof(T))} matching {doNotPopulateThisValue}");
		return Cast<T>(x => (T)(object)x!);
	}

	private AsyncSingleItemResult<TCollection, T> Cast<T>(Func<TItem?, T> memberAccessor)
		=> new(_expectationBuilder, new PredicateOptions<T>(),
			async x => memberAccessor(await _asyncMemberAccessor(x)));
}
