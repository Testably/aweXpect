using System.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class EventFilters
{
	/// <summary>
	///     Filters for events that have the given <paramref name="accessModifier" />.
	/// </summary>
	public static Filtered.Events WhichAre(this Filtered.Events @this,
		AccessModifiers accessModifier)
		=> @this.Which(Filter.Prefix<EventInfo>(
			type => type.HasAccessModifier(accessModifier),
			accessModifier.GetString(" ")));

	/// <summary>
	///     Filters for events that do not have the given <paramref name="accessModifier" />.
	/// </summary>
	public static Filtered.Events WhichAreNot(this Filtered.Events @this,
		AccessModifiers accessModifier)
		=> @this.Which(Filter.Prefix<EventInfo>(
			type => !type.HasAccessModifier(accessModifier),
			"non-" + accessModifier.GetString(" ")));
}
