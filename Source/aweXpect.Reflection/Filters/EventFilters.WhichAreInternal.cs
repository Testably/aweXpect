using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class EventFilters
{
	/// <summary>
	///     Filters for events that are internal.
	/// </summary>
	public static Filtered.Events WhichAreInternal(this Filtered.Events @this)
		=> @this.WhichAre(AccessModifiers.Internal);

	/// <summary>
	///     Filters for events that are not internal.
	/// </summary>
	public static Filtered.Events WhichAreNotInternal(this Filtered.Events @this)
		=> @this.WhichAreNot(AccessModifiers.Internal);
}
