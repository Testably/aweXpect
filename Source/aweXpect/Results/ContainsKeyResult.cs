using System;
using aweXpect.Core;

namespace aweXpect.Results;

/// <summary>
///     The result on a single dictionary value.
/// </summary>
/// <remarks>
///     <seealso cref="AndOrResult{TCollection, TThat}" />
/// </remarks>
public abstract class ContainsKeyResult<TCollection, TThat, TValue>
	: AndOrResult<TCollection, TThat>
{
	private protected ContainsKeyResult(ExpectationBuilder expectationBuilder, TThat returnValue)
		: base(expectationBuilder, returnValue)
	{
	}

	/// <summary>
	///     Further expectations on the selected value of the dictionary.
	/// </summary>
	public IThatSubject<TValue> WhoseValue
		=> new ThatSubject<TValue>(ExpectationBuilder.ForWhich((TCollection collection) => GetValue(collection),
			" whose value ",
			$"value of key {FormatKey()}",
			grammars => (grammars & ~ExpectationGrammars.Plural) | ExpectationGrammars.Introduced));

	private protected abstract TValue GetValue(TCollection collection);

	private protected abstract string FormatKey();

	/// <remarks>
	///     The key stays out of the public type, as no public member uses it. The <paramref name="valueAccessor" /> gets
	///     the <paramref name="key" /> as its argument, so that the accessor of the value is only bound to the key when
	///     <see cref="WhoseValue" /> is used.
	/// </remarks>
	internal sealed class ForKey<TKey>(
		ExpectationBuilder expectationBuilder,
		TThat returnValue,
		TKey key,
		Func<TCollection, TKey, TValue> valueAccessor)
		: ContainsKeyResult<TCollection, TThat, TValue>(expectationBuilder, returnValue)
	{
		private protected override TValue GetValue(TCollection collection) => valueAccessor(collection, key);

		private protected override string FormatKey() => Formatter.Format(key);
	}
}
