using System.Reflection;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class ConstructorFilters
{
	/// <summary>
	///     Filters for constructors that are static.
	/// </summary>
	public static Filtered.Constructors WhichAreStatic(this Filtered.Constructors @this)
		=> @this.Which(Filter.Prefix<ConstructorInfo>(
			constructor => constructor.IsStatic,
			"static "));

	/// <summary>
	///     Filters for constructors that are not static.
	/// </summary>
	public static Filtered.Constructors WhichAreNotStatic(this Filtered.Constructors @this)
		=> @this.Which(Filter.Prefix<ConstructorInfo>(
			constructor => !constructor.IsStatic,
			"non-static "));
}
