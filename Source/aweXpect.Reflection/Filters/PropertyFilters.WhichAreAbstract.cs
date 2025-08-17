using System.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class PropertyFilters
{
	/// <summary>
	///     Filters for properties that are abstract.
	/// </summary>
	public static Filtered.Properties WhichAreAbstract(this Filtered.Properties @this)
		=> @this.Which(Filter.Prefix<PropertyInfo>(
			property => property.IsReallyAbstract(),
			"abstract "));

	/// <summary>
	///     Filters for properties that are not abstract.
	/// </summary>
	public static Filtered.Properties WhichAreNotAbstract(this Filtered.Properties @this)
		=> @this.Which(Filter.Prefix<PropertyInfo>(
			property => !property.IsReallyAbstract(),
			"non-abstract "));
}
