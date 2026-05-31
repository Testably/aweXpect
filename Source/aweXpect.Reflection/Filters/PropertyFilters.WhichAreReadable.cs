using System.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class PropertyFilters
{
	/// <summary>
	///     Filters for properties that are readable (can be read).
	/// </summary>
	public static Filtered.Properties WhichAreReadable(this Filtered.Properties @this)
		=> @this.Which(Filter.Prefix<PropertyInfo>(
			property => property.IsReadable(),
			"readable "));
}
