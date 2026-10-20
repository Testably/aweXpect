using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class ConstructorFilters
{
	/// <summary>
	///     Filters for constructors that are internal.
	/// </summary>
	public static Filtered.Constructors WhichAreInternal(this Filtered.Constructors @this)
		=> @this.WhichAre(AccessModifiers.Internal);

	/// <summary>
	///     Filters for constructors that are not internal.
	/// </summary>
	public static Filtered.Constructors WhichAreNotInternal(this Filtered.Constructors @this)
		=> @this.WhichAreNot(AccessModifiers.Internal);
}
