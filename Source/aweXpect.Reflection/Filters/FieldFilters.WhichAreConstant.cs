using System.Reflection;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class FieldFilters
{
	/// <summary>
	///     Filters for fields that are constant.
	/// </summary>
	public static Filtered.Fields WhichAreConstant(this Filtered.Fields @this)
		=> @this.Which(Filter.Prefix<FieldInfo>(
			field => field.IsLiteral,
			"constant "));

	/// <summary>
	///     Filters for fields that are not constant.
	/// </summary>
	public static Filtered.Fields WhichAreNotConstant(this Filtered.Fields @this)
		=> @this.Which(Filter.Prefix<FieldInfo>(
			field => !field.IsLiteral,
			"non-constant "));
}
