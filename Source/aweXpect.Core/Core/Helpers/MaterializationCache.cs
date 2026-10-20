using System.Collections.Generic;

namespace aweXpect.Core.Helpers;

/// <summary>
///     The materializations of the source collections in an evaluation, one per source collection.
/// </summary>
/// <remarks>
///     The first materialization is held in fields, as an evaluation rarely materializes more than one collection and
///     a list would cost more than the materialization itself.
/// </remarks>
internal sealed class MaterializationCache(object source, object materialized)
{
	private readonly object _firstMaterialized = materialized;
	private readonly object _firstSource = source;
	private List<(object Source, object Materialized)>? _more;

	/// <summary>
	///     The number of materializations.
	/// </summary>
	public int Count => 1 + (_more?.Count ?? 0);

	/// <summary>
	///     Adds the <paramref name="materialized" /> copy of the <paramref name="source" />.
	/// </summary>
	public void Add(object source, object materialized)
		=> (_more ??= []).Add((source, materialized));

	/// <summary>
	///     Gets the source at the <paramref name="index" />.
	/// </summary>
	public object GetSource(int index)
		=> index == 0 ? _firstSource : _more![index - 1].Source;

	/// <summary>
	///     Gets the materialized copy at the <paramref name="index" />.
	/// </summary>
	public object GetMaterialized(int index)
		=> index == 0 ? _firstMaterialized : _more![index - 1].Materialized;
}
