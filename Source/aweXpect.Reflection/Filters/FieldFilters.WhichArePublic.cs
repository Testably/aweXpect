using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class FieldFilters
{
	/// <summary>
	///     Filters for fields that are public.
	/// </summary>
	public static Filtered.Fields WhichArePublic(this Filtered.Fields @this)
		=> @this.WhichAre(AccessModifiers.Public);

	/// <summary>
	///     Filters for fields that are not public.
	/// </summary>
	public static Filtered.Fields WhichAreNotPublic(this Filtered.Fields @this)
		=> @this.WhichAreNot(AccessModifiers.Public);
}
