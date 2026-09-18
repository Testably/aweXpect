using System.Collections.Generic;

namespace aweXpect.Helpers;

/// <summary>
///     A collection whose items belong to keys, so that a context lists the items together with their keys.
/// </summary>
internal interface IKeyedCollection
{
	/// <summary>
	///     Formats all items together with their keys.
	/// </summary>
	string Format();

	/// <summary>
	///     Formats the items at the <paramref name="indices" /> together with their keys.
	/// </summary>
	string Format(IEnumerable<int> indices);
}
