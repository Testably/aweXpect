using System.Reflection;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

/// <summary>
///     Extensions properties to filter <see cref="Filtered.Properties" />.
/// </summary>
public static partial class PropertyFilters
{
	/// <summary>
	///     Additional filters on properties of a specific type.
	/// </summary>
	public partial class PropertiesOfType(Filtered.Properties inner, IChangeableFilter<PropertyInfo> filter)
		: Filtered.Properties(inner);
}
