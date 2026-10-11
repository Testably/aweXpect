using System;
using System.Collections.Generic;
using aweXpect.Core;

namespace aweXpect.Results;

/// <summary>
///     The result on a set of dictionary values.
/// </summary>
/// <remarks>
///     <seealso cref="AndOrResult{TCollection, TThat}" />
/// </remarks>
public class ContainsKeysResult<TCollection, TThat, TKey, TValue>
	: AndOrResult<TCollection, TThat>
{
	private readonly TKey[] _keys;
	private readonly Func<TCollection, TKey[], IEnumerable<TValue>> _valuesAccessor;

	/// <remarks>
	///     The <paramref name="valuesAccessor" /> gets the <paramref name="keys" /> as its argument, so that the
	///     accessor of the values is only bound to the keys when <see cref="WhoseValues" /> is used.
	/// </remarks>
	internal ContainsKeysResult(
		ExpectationBuilder expectationBuilder,
		TThat returnValue,
		TKey[] keys,
		Func<TCollection, TKey[], IEnumerable<TValue>> valuesAccessor)
		: base(expectationBuilder, returnValue)
	{
		_keys = keys;
		_valuesAccessor = valuesAccessor;
	}

	/// <summary>
	///     Further expectations on the selected values of the dictionary.
	/// </summary>
	public IThat<IEnumerable<TValue>> WhoseValues
		=> new ThatSubject<IEnumerable<TValue>>(ExpectationBuilder
			.ForWhich((TCollection collection) => _valuesAccessor(collection, _keys), " whose values ", $"values of keys {Formatter.Format(_keys)}",
				grammars => grammars | ExpectationGrammars.Nested | ExpectationGrammars.Plural |
				            ExpectationGrammars.Introduced));
}
