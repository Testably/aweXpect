using System.Reflection;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class FieldFilters
{
	/// <summary>
	///     Filters for fields that are static.
	/// </summary>
	public static Filtered.Fields WhichAreStatic(this Filtered.Fields @this)
		=> @this.Which(Filter.Prefix<FieldInfo>(
			field => field.IsStatic,
			"static "));

	/// <summary>
	///     Filters for fields that are not static.
	/// </summary>
	public static Filtered.Fields WhichAreNotStatic(this Filtered.Fields @this)
		=> @this.Which(Filter.Prefix<FieldInfo>(
			field => !field.IsStatic,
			"non-static "));
}
