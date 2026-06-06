using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class MethodFilters
{
	/// <summary>
	///     Filter for methods which return <see langword="void" />.
	/// </summary>
	public static MethodsWhichReturn WhichReturnVoid(this Filtered.Methods @this)
		=> WhichReturn(@this, typeof(void));
}
