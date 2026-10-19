using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class MethodFilters
{
	/// <summary>
	///     Filters for methods that are internal.
	/// </summary>
	public static Filtered.Methods WhichAreInternal(this Filtered.Methods @this)
		=> @this.WhichAre(AccessModifiers.Internal);

	/// <summary>
	///     Filters for methods that are not internal.
	/// </summary>
	public static Filtered.Methods WhichAreNotInternal(this Filtered.Methods @this)
		=> @this.WhichAreNot(AccessModifiers.Internal);
}
