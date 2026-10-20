using System.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class MethodFilters
{
	/// <summary>
	///     Filters for methods that are obsolete (marked with the <see cref="System.ObsoleteAttribute" />).
	/// </summary>
	public static Filtered.Methods WhichAreObsolete(this Filtered.Methods @this)
		=> @this.Which(Filter.Prefix<MethodInfo>(
			method => method.IsObsolete(),
			"obsolete "));

	/// <summary>
	///     Filters for methods that are not obsolete (not marked with the <see cref="System.ObsoleteAttribute" />).
	/// </summary>
	public static Filtered.Methods WhichAreNotObsolete(this Filtered.Methods @this)
		=> @this.Which(Filter.Prefix<MethodInfo>(
			method => !method.IsObsolete(),
			"non-obsolete "));
}
