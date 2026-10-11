using System;
using aweXpect.Core;

namespace aweXpect.Results;

/// <summary>
///     The result on a single dictionary value.
/// </summary>
/// <remarks>
///     <seealso cref="AndOrResult{TCollection, TThat}" />
/// </remarks>
public class ContainsKeyResult<TCollection, TThat, TKey, TValue>
	: AndOrResult<TCollection, TThat>
{
	private readonly TKey _key;
	private readonly Func<TCollection, TKey, TValue> _valueAccessor;

	/// <remarks>
	///     The <paramref name="valueAccessor" /> gets the <paramref name="key" /> as its argument, so that the accessor of
	///     the value is only bound to the key when <see cref="WhoseValue" /> is used.
	/// </remarks>
	internal ContainsKeyResult(
		ExpectationBuilder expectationBuilder,
		TThat returnValue,
		TKey key,
		Func<TCollection, TKey, TValue> valueAccessor)
		: base(expectationBuilder, returnValue)
	{
		_key = key;
		_valueAccessor = valueAccessor;
	}

	/// <summary>
	///     Further expectations on the selected value of the dictionary.
	/// </summary>
	public IThat<TValue> WhoseValue
		=> new ThatSubject<TValue>(ExpectationBuilder.ForWhich((TCollection collection) => _valueAccessor(collection, _key),
			" whose value ",
			$"value of key {Formatter.Format(_key)}",
			grammars => (grammars & ~ExpectationGrammars.Plural) | ExpectationGrammars.Introduced));
}
