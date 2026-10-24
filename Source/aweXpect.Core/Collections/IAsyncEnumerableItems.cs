#if NET8_0_OR_GREATER
using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect;

/// <summary>
///     The items of an <see cref="IAsyncEnumerable{T}" /> of <typeparamref name="TItem" /> that a quantifier like
///     <c>All()</c> or <c>AtLeast(2)</c> selected, for an extension method that adds an expectation on them.
/// </summary>
/// <remarks>
///     The expectations on the items are extension methods on this interface. It is covariant, so that an expectation on
///     the items of a base type, or of a nullable type, applies to the items of a derived or not nullable type as well.
/// </remarks>
public interface IAsyncEnumerableItems<out TItem>
{
	/// <summary>
	///     The quantifier for the items.
	/// </summary>
	EnumerableQuantifier Quantifier { get; }

	/// <summary>
	///     The subject of the expectation.
	/// </summary>
	IThat<IAsyncEnumerable<TItem>?> Subject { get; }
}
#endif
