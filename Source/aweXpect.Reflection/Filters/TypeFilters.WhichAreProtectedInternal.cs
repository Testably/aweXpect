using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class TypeFilters
{
	/// <summary>
	///     Filters for types that are protected internal.
	/// </summary>
	public static Filtered.Types WhichAreProtectedInternal(this Filtered.Types @this)
		=> @this.WhichAre(AccessModifiers.ProtectedInternal);

	/// <summary>
	///     Filters for types that are not protected internal.
	/// </summary>
	public static Filtered.Types WhichAreNotProtectedInternal(this Filtered.Types @this)
		=> @this.WhichAreNot(AccessModifiers.ProtectedInternal);
}
