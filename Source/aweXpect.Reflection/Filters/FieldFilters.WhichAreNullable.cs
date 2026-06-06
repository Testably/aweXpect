using System.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class FieldFilters
{
	/// <summary>
	///     Filters for fields that are nullable.
	/// </summary>
	public static Filtered.Fields WhichAreNullable(this Filtered.Fields @this)
		=> @this.Which(Filter.Prefix<FieldInfo>(
			field => field.IsNullable(),
			"nullable "));

	/// <summary>
	///     Filters for fields that are not nullable.
	/// </summary>
	public static Filtered.Fields WhichAreNotNullable(this Filtered.Fields @this)
		=> @this.Which(Filter.Prefix<FieldInfo>(
			field => !field.IsNullable(),
			"non-nullable "));
}
