using System.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class PropertyFilters
{
	/// <summary>
	///     Filters for properties that are obsolete (marked with the <see cref="System.ObsoleteAttribute" />).
	/// </summary>
	public static Filtered.Properties WhichAreObsolete(this Filtered.Properties @this)
		=> @this.Which(Filter.Prefix<PropertyInfo>(
			property => property.IsObsolete(),
			"obsolete "));

	/// <summary>
	///     Filters for properties that are not obsolete (not marked with the <see cref="System.ObsoleteAttribute" />).
	/// </summary>
	public static Filtered.Properties WhichAreNotObsolete(this Filtered.Properties @this)
		=> @this.Which(Filter.Prefix<PropertyInfo>(
			property => !property.IsObsolete(),
			"non-obsolete "));
}
