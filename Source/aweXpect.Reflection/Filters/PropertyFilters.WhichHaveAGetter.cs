using System.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class PropertyFilters
{
	/// <summary>
	///     Filters for properties that have a getter.
	/// </summary>
	public static Filtered.Properties WhichHaveAGetter(this Filtered.Properties @this)
		=> @this.Which(Filter.Suffix<PropertyInfo>(
			property => property.HasGetter(),
			"with a getter "));
}
