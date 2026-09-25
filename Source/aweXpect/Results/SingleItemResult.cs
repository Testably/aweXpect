using System;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Helpers;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     The result for verifying that a collection contains a single item.
/// </summary>
/// <remarks>
///     <seealso cref="ExpectationResult{TType,TSelf}" />
/// </remarks>
public class SingleItemResult<TCollection, TItem>
	: ExpectationResult<TItem, SingleItemResult<TCollection, TItem>>,
		IOptionsProvider<PredicateOptions<TItem>>
{
	private readonly ExpectationBuilder _expectationBuilder;
	private readonly Func<TCollection, TItem?> _memberAccessor;
	private readonly PredicateOptions<TItem> _options;

	internal SingleItemResult(ExpectationBuilder expectationBuilder,
		PredicateOptions<TItem> options,
		Func<TCollection, TItem?> memberAccessor)
		: base(expectationBuilder)
	{
		_expectationBuilder = expectationBuilder;
		_options = options;
		_memberAccessor = memberAccessor;
	}

	/// <summary>
	///     Further expectations on the single <typeparamref name="TItem" />.
	/// </summary>
	public IThat<TItem> Which
		=> new ThatSubject<TItem>(_expectationBuilder.ForWhich(_memberAccessor, " that ",
			expectationGrammar: grammars => grammars & ~ExpectationGrammars.Plural));

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	PredicateOptions<TItem> IOptionsProvider<PredicateOptions<TItem>>.Options => _options;

	/// <summary>
	///     …that satisfies the <paramref name="predicate" />.
	/// </summary>
	public SingleItemResult<TCollection, TItem> Matching(Func<TItem, bool> predicate,
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
	public SingleItemResult<TCollection, T> Matching<T>()
	{
		_options.SetPredicate(item => item is T,
			$" of type {Formatter.Format(typeof(T))}");
		return Cast<T>(x => (T)(object)x!);
	}

	/// <summary>
	///     …of type <typeparamref name="T" /> that satisfies the <paramref name="predicate" />.
	/// </summary>
	public SingleItemResult<TCollection, T> Matching<T>(Func<T, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
	{
		predicate.ThrowIfNull();
		_options.SetPredicate(item => item is T typed && predicate(typed),
			$" of type {Formatter.Format(typeof(T))} matching {doNotPopulateThisValue}");
		return Cast<T>(x => (T)(object)x!);
	}

	private SingleItemResult<TCollection, T> Cast<T>(Func<TItem?, T> memberAccessor)
		=> new(_expectationBuilder, new PredicateOptions<T>(), x => memberAccessor(_memberAccessor(x)));
}
