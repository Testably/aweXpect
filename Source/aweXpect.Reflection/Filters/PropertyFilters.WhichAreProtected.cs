using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class PropertyFilters
{
	/// <summary>
	///     Filters for properties that are protected.
	/// </summary>
	public static Filtered.Properties WhichAreProtected(this Filtered.Properties @this)
		=> @this.WhichAre(AccessModifiers.Protected);

	/// <summary>
	///     Filters for properties that are not protected.
	/// </summary>
	public static Filtered.Properties WhichAreNotProtected(this Filtered.Properties @this)
		=> @this.WhichAreNot(AccessModifiers.Protected);
}
