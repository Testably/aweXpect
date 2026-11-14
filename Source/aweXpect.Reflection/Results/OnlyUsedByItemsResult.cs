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
	///     All items of the collection are read before the first one is checked, so that a quantifier cannot complete
	///     early.
	/// </remarks>
	public OnlyUsedByItemsResult<TType, TValue> OrByEachOther()
	{
		_options.Value = _options.Value.OrByEachOther();
		return this;
	}
}
