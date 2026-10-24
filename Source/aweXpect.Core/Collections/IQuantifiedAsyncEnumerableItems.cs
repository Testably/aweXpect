#if NET8_0_OR_GREATER
using System;
using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect;

/// <summary>
///     The items of an <see cref="IAsyncEnumerable{T}" /> of <typeparamref name="TItem" /> that a quantifier like
///     <c>All()</c> or <c>AtLeast(2)</c> returns.
/// </summary>
/// <remarks>
///     Only the type checks, whose type argument cannot be inferred, are members of this interface. They name the item
///     type in their result, so it is invariant: extend the covariant <see cref="IAsyncEnumerableItems{TItem}" />
///     instead.
/// </remarks>
public interface IQuantifiedAsyncEnumerableItems<TItem> : IAsyncEnumerableItems<TItem>
{
	/// <summary>
	///     …are of type <typeparamref name="TType" />.
	/// </summary>
	AndOrResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>> Are<TType>();

	/// <summary>
	///     …are of type <paramref name="type" />.
	/// </summary>
	AndOrResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>> Are(Type type);

	/// <summary>
	///     …are exactly of type <typeparamref name="TType" />.
	/// </summary>
	AndOrResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>> AreExactly<TType>();

	/// <summary>
	///     …are exactly of type <paramref name="type" />.
	/// </summary>
	AndOrResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>> AreExactly(Type type);
}
#endif
