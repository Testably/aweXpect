using System.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class PropertyFilters
{
	/// <summary>
	///     Filters for properties that are static.
	/// </summary>
	public static Filtered.Properties WhichAreStatic(this Filtered.Properties @this)
		=> @this.Which(Filter.Prefix<PropertyInfo>(
			property => property.IsReallyStatic(),
			"static "));

	/// <summary>
	///     Filters for properties that are not static.
	/// </summary>
	public static Filtered.Properties WhichAreNotStatic(this Filtered.Properties @this)
		=> @this.Which(Filter.Prefix<PropertyInfo>(
			property => !property.IsReallyStatic(),
			"non-static "));
}
