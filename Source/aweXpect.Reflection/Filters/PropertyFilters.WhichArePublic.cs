using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class PropertyFilters
{
	/// <summary>
	///     Filters for properties that are public.
	/// </summary>
	public static Filtered.Properties WhichArePublic(this Filtered.Properties @this)
		=> @this.WhichAre(AccessModifiers.Public);

	/// <summary>
	///     Filters for properties that are not public.
	/// </summary>
	public static Filtered.Properties WhichAreNotPublic(this Filtered.Properties @this)
		=> @this.WhichAreNot(AccessModifiers.Public);
}
