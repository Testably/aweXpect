using System.Reflection;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class FieldFilters
{
	/// <summary>
	///     Filters for fields that are read-only.
	/// </summary>
	public static Filtered.Fields WhichAreReadOnly(this Filtered.Fields @this)
		=> @this.Which(Filter.Prefix<FieldInfo>(
			field => field.IsInitOnly,
			"read-only "));

	/// <summary>
	///     Filters for fields that are not read-only.
	/// </summary>
	public static Filtered.Fields WhichAreNotReadOnly(this Filtered.Fields @this)
		=> @this.Which(Filter.Prefix<FieldInfo>(
			field => !field.IsInitOnly,
			"non-read-only "));
}
