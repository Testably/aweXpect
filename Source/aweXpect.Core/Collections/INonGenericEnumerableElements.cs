using System.Collections;
using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect;

/// <summary>
///     The elements of a non-generic <see cref="IEnumerable" /> that a quantifier like <c>All()</c> or
///     <c>AtLeast(2)</c> selected, for an extension method that adds an expectation on them.
/// </summary>
public interface INonGenericEnumerableElements<out TEnumerable>
	where TEnumerable : IEnumerable?
{
	/// <summary>
	///     The quantifier for the elements.
	/// </summary>
	EnumerableQuantifier Quantifier { get; }

	/// <summary>
	///     The subject of the expectation.
	/// </summary>
	IThat<TEnumerable?> Subject { get; }
}
