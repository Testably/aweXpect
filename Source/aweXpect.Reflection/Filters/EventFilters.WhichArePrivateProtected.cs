using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class EventFilters
{
	/// <summary>
	///     Filters for events that are private protected.
	/// </summary>
	public static Filtered.Events WhichArePrivateProtected(this Filtered.Events @this)
		=> @this.WhichAre(AccessModifiers.PrivateProtected);

	/// <summary>
	///     Filters for events that are not private protected.
	/// </summary>
	public static Filtered.Events WhichAreNotPrivateProtected(this Filtered.Events @this)
		=> @this.WhichAreNot(AccessModifiers.PrivateProtected);
}
