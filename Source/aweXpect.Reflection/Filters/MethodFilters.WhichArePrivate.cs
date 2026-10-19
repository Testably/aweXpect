using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class MethodFilters
{
	/// <summary>
	///     Filters for methods that are private.
	/// </summary>
	public static Filtered.Methods WhichArePrivate(this Filtered.Methods @this)
		=> @this.WhichAre(AccessModifiers.Private);

	/// <summary>
	///     Filters for methods that are not private.
	/// </summary>
	public static Filtered.Methods WhichAreNotPrivate(this Filtered.Methods @this)
		=> @this.WhichAreNot(AccessModifiers.Private);
}
