using System.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class PropertyFilters
{
	/// <summary>
	///     Filters for properties that have a (regular, non-init) setter.
	/// </summary>
	public static Filtered.Properties WhichHaveASetter(this Filtered.Properties @this)
		=> @this.Which(Filter.Suffix<PropertyInfo>(
			property => property.HasSetter(),
			"with a setter "));
}
