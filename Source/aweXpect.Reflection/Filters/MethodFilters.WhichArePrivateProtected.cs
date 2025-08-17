using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class MethodFilters
{
	/// <summary>
	///     Filters for methods that are private protected.
	/// </summary>
	public static Filtered.Methods WhichArePrivateProtected(this Filtered.Methods @this)
		=> @this.WhichAre(AccessModifiers.PrivateProtected);

	/// <summary>
	///     Filters for methods that are not private protected.
	/// </summary>
	public static Filtered.Methods WhichAreNotPrivateProtected(this Filtered.Methods @this)
		=> @this.WhichAreNot(AccessModifiers.PrivateProtected);
}
