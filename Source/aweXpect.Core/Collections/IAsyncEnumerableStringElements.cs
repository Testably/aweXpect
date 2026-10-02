#if NET8_0_OR_GREATER
using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect;

/// <summary>
///     The elements of an <see cref="IAsyncEnumerable{T}" /> of <see langword="string" /> that a quantifier like
///     <c>All()</c> or <c>AtLeast(2)</c> selected, for an extension method that adds an expectation on them.
/// </summary>
public interface IAsyncEnumerableStringElements
{
	/// <summary>
	///     The quantifier for the elements.
	/// </summary>
	EnumerableQuantifier Quantifier { get; }

	/// <summary>
	///     The subject of the expectation.
	/// </summary>
	IThat<IAsyncEnumerable<string?>?> Subject { get; }
}
#endif
