using System.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class PropertyFilters
{
	/// <summary>
	///     Filters for properties that are nullable.
	/// </summary>
	public static Filtered.Properties WhichAreNullable(this Filtered.Properties @this)
		=> @this.Which(Filter.Prefix<PropertyInfo>(
			property => property.IsNullable(),
			"nullable "));

	/// <summary>
	///     Filters for properties that are not nullable.
	/// </summary>
	public static Filtered.Properties WhichAreNotNullable(this Filtered.Properties @this)
		=> @this.Which(Filter.Prefix<PropertyInfo>(
			property => !property.IsNullable(),
			"non-nullable "));
}
