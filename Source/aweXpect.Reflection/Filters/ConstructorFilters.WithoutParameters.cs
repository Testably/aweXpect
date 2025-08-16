using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class ConstructorFilters
{
	/// <summary>
	///     Filter for constructors without parameters.
	/// </summary>
	public static Filtered.Constructors WithoutParameters(this Filtered.Constructors @this)
		=> @this.WithParameterCount(0);
}
