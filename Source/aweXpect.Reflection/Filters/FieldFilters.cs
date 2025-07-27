using System.Reflection;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

/// <summary>
///     Extensions fields to filter <see cref="Filtered.Fields" />.
/// </summary>
public static partial class FieldFilters
{
	/// <summary>
	///     Additional filters on fields of a specific type.
	/// </summary>
	public partial class FieldsOfType(Filtered.Fields inner, IChangeableFilter<FieldInfo> filter)
		: Filtered.Fields(inner);
}
