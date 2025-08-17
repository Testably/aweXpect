using System.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class PropertyFilters
{
	/// <summary>
	///     Filters for properties that are sealed.
	/// </summary>
	public static Filtered.Properties WhichAreSealed(this Filtered.Properties @this)
		=> @this.Which(Filter.Prefix<PropertyInfo>(
			property => property.IsReallySealed(),
			"sealed "));

	/// <summary>
	///     Filters for properties that are not sealed.
	/// </summary>
	public static Filtered.Properties WhichAreNotSealed(this Filtered.Properties @this)
		=> @this.Which(Filter.Prefix<PropertyInfo>(
			property => !property.IsReallySealed(),
			"non-sealed "));
}
