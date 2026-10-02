using System.Collections.Generic;

namespace aweXpect.Formatting;

/// <summary>
///     A collection whose items belong to keys, e.g. the values that an expectation on a dictionary passes on.
/// </summary>
/// <remarks>
///     The contexts of a failure, e.g. the matching items of a quantified expectation, list its items together with
///     their keys.
/// </remarks>
public interface IKeyedCollection
{
	/// <summary>
	///     Formats all items together with their keys.
	/// </summary>
	string Format();

	/// <summary>
	///     Formats the items at the <paramref name="indices" /> together with their keys, of which
	///     <paramref name="totalCount" /> were found in total.
	/// </summary>
	string Format(IEnumerable<int> indices, int? totalCount);
}
