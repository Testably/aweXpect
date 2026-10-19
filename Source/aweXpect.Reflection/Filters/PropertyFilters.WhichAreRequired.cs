using System.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class PropertyFilters
{
	/// <summary>
	///     Filters for properties that are required.
	/// </summary>
	public static Filtered.Properties WhichAreRequired(this Filtered.Properties @this)
		=> @this.Which(Filter.Prefix<PropertyInfo>(
			property => property.IsRequired(),
			"required "));

	/// <summary>
	///     Filters for properties that are not required.
	/// </summary>
	public static Filtered.Properties WhichAreNotRequired(this Filtered.Properties @this)
		=> @this.Which(Filter.Prefix<PropertyInfo>(
			property => !property.IsRequired(),
			"non-required "));
}
