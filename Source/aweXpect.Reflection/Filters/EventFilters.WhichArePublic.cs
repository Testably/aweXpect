using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class EventFilters
{
	/// <summary>
	///     Filters for events that are public.
	/// </summary>
	public static Filtered.Events WhichArePublic(this Filtered.Events @this)
		=> @this.WhichAre(AccessModifiers.Public);

	/// <summary>
	///     Filters for events that are not public.
	/// </summary>
	public static Filtered.Events WhichAreNotPublic(this Filtered.Events @this)
		=> @this.WhichAreNot(AccessModifiers.Public);
}
