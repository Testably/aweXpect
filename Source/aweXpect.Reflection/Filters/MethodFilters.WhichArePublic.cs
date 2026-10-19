using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class MethodFilters
{
	/// <summary>
	///     Filters for methods that are public.
	/// </summary>
	public static Filtered.Methods WhichArePublic(this Filtered.Methods @this)
		=> @this.WhichAre(AccessModifiers.Public);

	/// <summary>
	///     Filters for methods that are not public.
	/// </summary>
	public static Filtered.Methods WhichAreNotPublic(this Filtered.Methods @this)
		=> @this.WhichAreNot(AccessModifiers.Public);
}
