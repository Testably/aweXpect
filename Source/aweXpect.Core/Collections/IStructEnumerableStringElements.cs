using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect;

/// <summary>
///     The elements of a struct collection of <see langword="string" />, e.g. an <c>ImmutableArray</c>, that a
///     quantifier like <c>All()</c> or <c>AtLeast(2)</c> selected, for an extension method that adds an expectation on
///     them.
/// </summary>
public interface IStructEnumerableStringElements<out TEnumerable>
	where TEnumerable : struct, IEnumerable<string?>
{
	/// <summary>
	///     The quantifier for the elements.
	/// </summary>
	EnumerableQuantifier Quantifier { get; }

	/// <summary>
	///     The subject of the expectation.
	/// </summary>
	IThat<TEnumerable> Subject { get; }
}
