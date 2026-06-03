using System.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class PropertyFilters
{
	/// <summary>
	///     Filters for properties that override a base class property.
	/// </summary>
	public static Filtered.Properties WhichOverride(this Filtered.Properties @this)
		=> @this.Which(Filter.Suffix<PropertyInfo>(
			property => property.IsOverride(),
			"which override a base property "));

	/// <summary>
	///     Filters for properties that do not override a base class property.
	/// </summary>
	public static Filtered.Properties WhichDoNotOverride(this Filtered.Properties @this)
		=> @this.Which(Filter.Suffix<PropertyInfo>(
			property => !property.IsOverride(),
			"which do not override a base property "));
}
