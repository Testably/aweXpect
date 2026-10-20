using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class TypeFilters
{
	/// <summary>
	///     Filters for types that are public.
	/// </summary>
	public static Filtered.Types WhichArePublic(this Filtered.Types @this)
		=> @this.WhichAre(AccessModifiers.Public);

	/// <summary>
	///     Filters for types that are not public.
	/// </summary>
	public static Filtered.Types WhichAreNotPublic(this Filtered.Types @this)
		=> @this.WhichAreNot(AccessModifiers.Public);
}
