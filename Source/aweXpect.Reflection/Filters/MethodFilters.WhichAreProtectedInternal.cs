using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class MethodFilters
{
	/// <summary>
	///     Filters for methods that are protected internal.
	/// </summary>
	public static Filtered.Methods WhichAreProtectedInternal(this Filtered.Methods @this)
		=> @this.WhichAre(AccessModifiers.ProtectedInternal);

	/// <summary>
	///     Filters for methods that are not protected internal.
	/// </summary>
	public static Filtered.Methods WhichAreNotProtectedInternal(this Filtered.Methods @this)
		=> @this.WhichAreNot(AccessModifiers.ProtectedInternal);
}
