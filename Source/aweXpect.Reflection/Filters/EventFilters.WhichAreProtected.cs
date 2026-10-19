using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class EventFilters
{
	/// <summary>
	///     Filters for events that are protected.
	/// </summary>
	public static Filtered.Events WhichAreProtected(this Filtered.Events @this)
		=> @this.WhichAre(AccessModifiers.Protected);

	/// <summary>
	///     Filters for events that are not protected.
	/// </summary>
	public static Filtered.Events WhichAreNotProtected(this Filtered.Events @this)
		=> @this.WhichAreNot(AccessModifiers.Protected);
}
