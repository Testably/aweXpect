using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class PropertyFilters
{
	/// <summary>
	///     Filters for properties that are private protected.
	/// </summary>
	public static Filtered.Properties WhichArePrivateProtected(this Filtered.Properties @this)
		=> @this.WhichAre(AccessModifiers.PrivateProtected);

	/// <summary>
	///     Filters for properties that are not private protected.
	/// </summary>
	public static Filtered.Properties WhichAreNotPrivateProtected(this Filtered.Properties @this)
		=> @this.WhichAreNot(AccessModifiers.PrivateProtected);
}
