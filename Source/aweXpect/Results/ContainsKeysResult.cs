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
public abstract class ContainsKeysResult<TCollection, TThat, TValue>
	: AndOrResult<TCollection, TThat>
{
	private protected ContainsKeysResult(ExpectationBuilder expectationBuilder, TThat returnValue)
		: base(expectationBuilder, returnValue)
	{
	}

	/// <summary>
	///     Further expectations on the selected values of the dictionary.
	/// </summary>
	public IThatSubject<IEnumerable<TValue>> WhoseValues
		=> new ThatSubject<IEnumerable<TValue>>(ExpectationBuilder
			.ForWhich((TCollection collection) => GetValues(collection), " whose values ", $"values of keys {FormatKeys()}",
				grammars => grammars | ExpectationGrammars.Nested | ExpectationGrammars.Plural |
				            ExpectationGrammars.Introduced));

	private protected abstract IEnumerable<TValue> GetValues(TCollection collection);

	private protected abstract string FormatKeys();

	/// <remarks>
	///     The keys stay out of the public type, as no public member uses them. The <paramref name="valuesAccessor" />
	///     gets the <paramref name="keys" /> as its argument, so that the accessor of the values is only bound to the
	///     keys when <see cref="WhoseValues" /> is used.
	/// </remarks>
	internal sealed class ForKeys<TKey>(
		ExpectationBuilder expectationBuilder,
		TThat returnValue,
		TKey[] keys,
		Func<TCollection, TKey[], IEnumerable<TValue>> valuesAccessor)
		: ContainsKeysResult<TCollection, TThat, TValue>(expectationBuilder, returnValue)
	{
		private protected override IEnumerable<TValue> GetValues(TCollection collection)
			=> valuesAccessor(collection, keys);

		private protected override string FormatKeys() => Formatter.Format(keys);
	}
}
