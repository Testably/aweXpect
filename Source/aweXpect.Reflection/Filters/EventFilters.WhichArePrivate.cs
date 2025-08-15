using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class EventFilters
{
	/// <summary>
	///     Filters for events that are private.
	/// </summary>
	public static Filtered.Events WhichArePrivate(this Filtered.Events @this)
		=> @this.WhichAre(AccessModifiers.Private);

	/// <summary>
	///     Filters for events that are not private.
	/// </summary>
	public static Filtered.Events WhichAreNotPrivate(this Filtered.Events @this)
		=> @this.WhichAreNot(AccessModifiers.Private);
}
