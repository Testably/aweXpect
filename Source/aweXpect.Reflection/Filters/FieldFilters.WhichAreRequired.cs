using System.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class FieldFilters
{
	/// <summary>
	///     Filters for fields that are required.
	/// </summary>
	public static Filtered.Fields WhichAreRequired(this Filtered.Fields @this)
		=> @this.Which(Filter.Prefix<FieldInfo>(
			field => field.IsRequired(),
			"required "));

	/// <summary>
	///     Filters for fields that are not required.
	/// </summary>
	public static Filtered.Fields WhichAreNotRequired(this Filtered.Fields @this)
		=> @this.Which(Filter.Prefix<FieldInfo>(
			field => !field.IsRequired(),
			"non-required "));
}
