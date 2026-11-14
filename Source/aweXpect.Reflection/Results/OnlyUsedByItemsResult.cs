using aweXpect.Core;
using aweXpect.Reflection.Options;
using aweXpect.Results;

namespace aweXpect.Reflection.Results;

/// <summary>
///     The result of an only-used-by assertion on the items of a collection, allowing the items to use each other.
/// </summary>
public sealed class OnlyUsedByItemsResult<TType, TValue>
	: AndOrResult<TType, IThat<TValue>, OnlyUsedByItemsResult<TType, TValue>>
{
	private readonly OptionsHolder<TypeUsageOptions> _options;

	internal OnlyUsedByItemsResult(
		ExpectationBuilder expectationBuilder,
		IThat<TValue> subject,
		OptionsHolder<TypeUsageOptions> options)
		: base(expectationBuilder, subject)
	{
		_options = options;
	}

	/// <summary>
	///     Also allows the items of the collection to use each other.
	/// </summary>
	/// <remarks>
	///     The collection is read to its end before its first item is checked, also when a quantifier needs fewer
	///     items, so an endless collection never completes and an exception of the collection fails the expectation.
	/// </remarks>
	public OnlyUsedByItemsResult<TType, TValue> OrByEachOther()
	{
		_options.Value = _options.Value.OrByEachOther();
		return this;
	}
}
