using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class PropertyFilters
{
	/// <summary>
	///     Filters for properties that are protected internal.
	/// </summary>
	public static Filtered.Properties WhichAreProtectedInternal(this Filtered.Properties @this)
		=> @this.WhichAre(AccessModifiers.ProtectedInternal);

	/// <summary>
	///     Filters for properties that are not protected internal.
	/// </summary>
	public static Filtered.Properties WhichAreNotProtectedInternal(this Filtered.Properties @this)
		=> @this.WhichAreNot(AccessModifiers.ProtectedInternal);
}
