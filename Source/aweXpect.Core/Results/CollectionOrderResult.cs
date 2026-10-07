using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     The result of an expectation with an underlying value of type <typeparamref name="TType" />.
///     <para />
///     In addition to the combinations from <see cref="AndOrResult{TType,TThat}" />, allows specifying
///     the <see cref="IComparer{TMember}" /> used to verify the order of items.
/// </summary>
public class CollectionOrderResult<TMember, TType, TThat>(
	ExpectationBuilder expectationBuilder,
	TThat returnValue,
	CollectionOrderOptions<TMember> options)
	: AndOrResult<TType, TThat>(expectationBuilder, returnValue),
		IOptionsProvider<CollectionOrderOptions<TMember>>
{
	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	CollectionOrderOptions<TMember> IOptionsProvider<CollectionOrderOptions<TMember>>.Options => options;

	/// <summary>
	///     Use the given <paramref name="comparer" /> to verify the order of items.
	/// </summary>
	/// <exception cref="System.InvalidOperationException">A comparer is already set.</exception>
	public AndOrResult<TType, TThat> Using(IComparer<TMember> comparer)
	{
		options.SetComparer(comparer);
		return this;
	}
}
