using System.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class FieldFilters
{
	/// <summary>
	///     Filters for fields that are obsolete (marked with the <see cref="System.ObsoleteAttribute" />).
	/// </summary>
	public static Filtered.Fields WhichAreObsolete(this Filtered.Fields @this)
		=> @this.Which(Filter.Prefix<FieldInfo>(
			field => field.IsObsolete(),
			"obsolete "));

	/// <summary>
	///     Filters for fields that are not obsolete (not marked with the <see cref="System.ObsoleteAttribute" />).
	/// </summary>
	public static Filtered.Fields WhichAreNotObsolete(this Filtered.Fields @this)
		=> @this.Which(Filter.Prefix<FieldInfo>(
			field => !field.IsObsolete(),
			"non-obsolete "));
}
