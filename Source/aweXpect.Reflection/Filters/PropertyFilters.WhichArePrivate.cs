using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class PropertyFilters
{
	/// <summary>
	///     Filters for properties that are private.
	/// </summary>
	public static Filtered.Properties WhichArePrivate(this Filtered.Properties @this)
		=> @this.WhichAre(AccessModifiers.Private);

	/// <summary>
	///     Filters for properties that are not private.
	/// </summary>
	public static Filtered.Properties WhichAreNotPrivate(this Filtered.Properties @this)
		=> @this.WhichAreNot(AccessModifiers.Private);
}
