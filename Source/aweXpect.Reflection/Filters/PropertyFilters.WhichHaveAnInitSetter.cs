using System.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class PropertyFilters
{
	/// <summary>
	///     Filters for properties that have an init-only setter.
	/// </summary>
	public static Filtered.Properties WhichHaveAnInitSetter(this Filtered.Properties @this)
		=> @this.Which(Filter.Suffix<PropertyInfo>(
			property => property.HasInitSetter(),
			"with an init setter "));
}
