using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class TypeFilters
{
	/// <summary>
	///     Filters for types that are private protected.
	/// </summary>
	public static Filtered.Types WhichArePrivateProtected(this Filtered.Types @this)
		=> @this.WhichAre(AccessModifiers.PrivateProtected);

	/// <summary>
	///     Filters for types that are not private protected.
	/// </summary>
	public static Filtered.Types WhichAreNotPrivateProtected(this Filtered.Types @this)
		=> @this.WhichAreNot(AccessModifiers.PrivateProtected);
}
