using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class MethodFilters
{
	/// <summary>
	///     Filter for methods which return <see langword="void" />.
	/// </summary>
	public static MethodsWhichReturn WhichReturnVoid(this Filtered.Methods @this)
		=> WhichReturn(@this, typeof(void));

	/// <summary>
	///     Filter for methods which do not return <see langword="void" />.
	/// </summary>
	public static Filtered.Methods WhichDoNotReturnVoid(this Filtered.Methods @this)
		=> WhichDoNotReturn(@this, typeof(void));
}
