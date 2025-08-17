using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class EventFilters
{
	/// <summary>
	///     Filters for events that are protected internal.
	/// </summary>
	public static Filtered.Events WhichAreProtectedInternal(this Filtered.Events @this)
		=> @this.WhichAre(AccessModifiers.ProtectedInternal);

	/// <summary>
	///     Filters for events that are not protected internal.
	/// </summary>
	public static Filtered.Events WhichAreNotProtectedInternal(this Filtered.Events @this)
		=> @this.WhichAreNot(AccessModifiers.ProtectedInternal);
}
