using System;
using System.Threading.Tasks;
using aweXpect.Core;

namespace aweXpect.Results;

/// <summary>
///     The result for verifying that an asynchronous collection contains a single item matching a condition.
/// </summary>
/// <remarks>
///     <seealso cref="ExpectationResult{TType,TSelf}" />
/// </remarks>
public class AsyncSingleMatchingItemResult<TCollection, TItem>
	: ExpectationResult<TItem, AsyncSingleMatchingItemResult<TCollection, TItem>>
{
	private readonly Func<TCollection, Task<TItem?>> _asyncMemberAccessor;

	internal AsyncSingleMatchingItemResult(ExpectationBuilder expectationBuilder,
		Func<TCollection, Task<TItem?>> asyncMemberAccessor)
		: base(expectationBuilder)
	{
		_asyncMemberAccessor = asyncMemberAccessor;
	}

	/// <summary>
	///     Further expectations on the single item.
	/// </summary>
	public IThat<TItem> Which
		=> new ThatSubject<TItem>(ExpectationBuilder.ForWhich(_asyncMemberAccessor, " that "));
}
