using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class PropertyFilters
{
	/// <summary>
	///     Filters for properties that are internal.
	/// </summary>
	public static Filtered.Properties WhichAreInternal(this Filtered.Properties @this)
		=> @this.WhichAre(AccessModifiers.Internal);

	/// <summary>
	///     Filters for properties that are not internal.
	/// </summary>
	public static Filtered.Properties WhichAreNotInternal(this Filtered.Properties @this)
		=> @this.WhichAreNot(AccessModifiers.Internal);
}
