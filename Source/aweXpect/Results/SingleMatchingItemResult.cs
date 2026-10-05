using System;
using aweXpect.Core;

namespace aweXpect.Results;

/// <summary>
///     The result for verifying that a collection contains a single item matching a condition.
/// </summary>
/// <remarks>
///     <seealso cref="ExpectationResult{TType,TSelf}" />
/// </remarks>
public class SingleMatchingItemResult<TCollection, TItem>
	: ExpectationResult<TItem, SingleMatchingItemResult<TCollection, TItem>>
{
	private readonly Func<TCollection, TItem?> _memberAccessor;

	internal SingleMatchingItemResult(ExpectationBuilder expectationBuilder,
		Func<TCollection, TItem?> memberAccessor)
		: base(expectationBuilder)
	{
		_memberAccessor = memberAccessor;
	}

	/// <summary>
	///     Further expectations on the single <typeparamref name="TItem" />.
	/// </summary>
	public IThat<TItem> Which
		=> new ThatSubject<TItem>(ExpectationBuilder.ForWhich(_memberAccessor, " that ", "it",
			grammars => grammars & ~ExpectationGrammars.Plural));
}
