using System.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class FieldFilters
{
	/// <summary>
	///     Filters for fields that have the given <paramref name="accessModifier" />.
	/// </summary>
	public static Filtered.Fields WhichAre(this Filtered.Fields @this,
		AccessModifiers accessModifier)
		=> @this.Which(Filter.Prefix<FieldInfo>(
			type => type.HasAccessModifier(accessModifier),
			accessModifier.GetString(" ")));

	/// <summary>
	///     Filters for fields that do not have the given <paramref name="accessModifier" />.
	/// </summary>
	public static Filtered.Fields WhichAreNot(this Filtered.Fields @this,
		AccessModifiers accessModifier)
		=> @this.Which(Filter.Prefix<FieldInfo>(
			type => !type.HasAccessModifier(accessModifier),
			"non-" + accessModifier.GetString(" ")));
}
