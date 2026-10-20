using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class MethodFilters
{
	/// <summary>
	///     Filters for methods that are protected.
	/// </summary>
	public static Filtered.Methods WhichAreProtected(this Filtered.Methods @this)
		=> @this.WhichAre(AccessModifiers.Protected);

	/// <summary>
	///     Filters for methods that are not protected.
	/// </summary>
	public static Filtered.Methods WhichAreNotProtected(this Filtered.Methods @this)
		=> @this.WhichAreNot(AccessModifiers.Protected);
}
