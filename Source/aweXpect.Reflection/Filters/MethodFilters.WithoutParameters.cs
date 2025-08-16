using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class MethodFilters
{
	/// <summary>
	///     Filter for methods without parameters.
	/// </summary>
	public static Filtered.Methods WithoutParameters(this Filtered.Methods @this)
		=> @this.WithParameterCount(0);
}
