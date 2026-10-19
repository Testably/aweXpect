using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class TypeFilters
{
	/// <summary>
	///     Filters for types that are protected.
	/// </summary>
	public static Filtered.Types WhichAreProtected(this Filtered.Types @this)
		=> @this.WhichAre(AccessModifiers.Protected);

	/// <summary>
	///     Filters for types that are not protected.
	/// </summary>
	public static Filtered.Types WhichAreNotProtected(this Filtered.Types @this)
		=> @this.WhichAreNot(AccessModifiers.Protected);
}
