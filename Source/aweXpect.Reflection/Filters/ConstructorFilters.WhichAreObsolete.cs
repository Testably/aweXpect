using System.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class ConstructorFilters
{
	/// <summary>
	///     Filters for constructors that are obsolete (marked with the <see cref="System.ObsoleteAttribute" />).
	/// </summary>
	public static Filtered.Constructors WhichAreObsolete(this Filtered.Constructors @this)
		=> @this.Which(Filter.Prefix<ConstructorInfo>(
			constructor => constructor.IsObsolete(),
			"obsolete "));

	/// <summary>
	///     Filters for constructors that are not obsolete (not marked with the <see cref="System.ObsoleteAttribute" />).
	/// </summary>
	public static Filtered.Constructors WhichAreNotObsolete(this Filtered.Constructors @this)
		=> @this.Which(Filter.Prefix<ConstructorInfo>(
			constructor => !constructor.IsObsolete(),
			"non-obsolete "));
}
