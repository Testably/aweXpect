using System.Threading.Tasks;

namespace aweXpect.Core.Helpers;

/// <summary>
///     A lazily materialized copy of a source collection, whose items are read by all constraints and by the failure
///     message of an evaluation.
/// </summary>
internal interface IMaterialization
{
	/// <summary>
	///     Disposes the enumerator of the source, once the evaluation and its failure message no longer read from it.
	/// </summary>
	/// <remarks>
	///     Afterward, an enumeration only replays the items that were already read, and the count stays unknown.
	/// </remarks>
	Task ReleaseSource();
}
