using System.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class MethodFilters
{
	/// <summary>
	///     Filters for methods that have the given <paramref name="accessModifier" />.
	/// </summary>
	public static Filtered.Methods WhichAre(this Filtered.Methods @this,
		AccessModifiers accessModifier)
		=> @this.Which(Filter.Prefix<MethodInfo>(
			type => type.HasAccessModifier(accessModifier),
			accessModifier.GetString(" ")));

	/// <summary>
	///     Filters for methods that do not have the given <paramref name="accessModifier" />.
	/// </summary>
	public static Filtered.Methods WhichAreNot(this Filtered.Methods @this,
		AccessModifiers accessModifier)
		=> @this.Which(Filter.Prefix<MethodInfo>(
			type => !type.HasAccessModifier(accessModifier),
			"non-" + accessModifier.GetString(" ")));
}
