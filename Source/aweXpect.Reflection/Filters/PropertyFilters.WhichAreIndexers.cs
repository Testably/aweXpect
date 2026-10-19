using System.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class PropertyFilters
{
	/// <summary>
	///     Filters for properties that are indexers (have index parameters).
	/// </summary>
	public static Filtered.Properties WhichAreIndexers(this Filtered.Properties @this)
		=> @this.Which(Filter.Prefix<PropertyInfo>(
			property => property.IsIndexer(),
			"indexer "));

	/// <summary>
	///     Filters for properties that are not indexers (have no index parameters).
	/// </summary>
	public static Filtered.Properties WhichAreNotIndexers(this Filtered.Properties @this)
		=> @this.Which(Filter.Prefix<PropertyInfo>(
			property => !property.IsIndexer(),
			"non-indexer "));
}
