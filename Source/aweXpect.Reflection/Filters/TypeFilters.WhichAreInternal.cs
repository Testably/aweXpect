using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class TypeFilters
{
	/// <summary>
	///     Filters for types that are internal.
	/// </summary>
	public static Filtered.Types WhichAreInternal(this Filtered.Types @this)
		=> @this.WhichAre(AccessModifiers.Internal);

	/// <summary>
	///     Filters for types that are not internal.
	/// </summary>
	public static Filtered.Types WhichAreNotInternal(this Filtered.Types @this)
		=> @this.WhichAreNot(AccessModifiers.Internal);
}
