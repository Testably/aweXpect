using System.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class PropertyFilters
{
	/// <summary>
	///     Filters for properties that are virtual.
	/// </summary>
	public static Filtered.Properties WhichAreVirtual(this Filtered.Properties @this)
		=> @this.Which(Filter.Prefix<PropertyInfo>(
			property => property.IsReallyVirtual(),
			"virtual "));

	/// <summary>
	///     Filters for properties that are not virtual.
	/// </summary>
	public static Filtered.Properties WhichAreNotVirtual(this Filtered.Properties @this)
		=> @this.Which(Filter.Prefix<PropertyInfo>(
			property => !property.IsReallyVirtual(),
			"non-virtual "));
}
