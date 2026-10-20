using System.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class PropertyFilters
{
	/// <summary>
	///     Filters for properties that have the given <paramref name="accessModifier" />.
	/// </summary>
	public static Filtered.Properties WhichAre(this Filtered.Properties @this,
		AccessModifiers accessModifier)
		=> @this.Which(Filter.Prefix<PropertyInfo>(
			type => type.HasAccessModifier(accessModifier),
			accessModifier.GetString(" ")));

	/// <summary>
	///     Filters for properties that do not have the given <paramref name="accessModifier" />.
	/// </summary>
	public static Filtered.Properties WhichAreNot(this Filtered.Properties @this,
		AccessModifiers accessModifier)
		=> @this.Which(Filter.Prefix<PropertyInfo>(
			type => !type.HasAccessModifier(accessModifier),
			"non-" + accessModifier.GetString(" ")));
}
