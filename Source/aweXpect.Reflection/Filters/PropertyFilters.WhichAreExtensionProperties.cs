using System.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class PropertyFilters
{
	/// <summary>
	///     Filters for properties that are extension properties (declared with the C# extension block syntax).
	/// </summary>
	public static Filtered.Properties WhichAreExtensionProperties(this Filtered.Properties @this)
		=> @this.Which(Filter.Prefix<PropertyInfo>(
			property => property.IsExtensionProperty(),
			"extension "));

	/// <summary>
	///     Filters for properties that are not extension properties (not declared with the C# extension block syntax).
	/// </summary>
	public static Filtered.Properties WhichAreNotExtensionProperties(this Filtered.Properties @this)
		=> @this.Which(Filter.Prefix<PropertyInfo>(
			property => !property.IsExtensionProperty(),
			"non-extension "));
}
