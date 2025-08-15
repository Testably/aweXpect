using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class ConstructorFilters
{
	/// <summary>
	///     Filters for constructors that are protected.
	/// </summary>
	public static Filtered.Constructors WhichAreProtected(this Filtered.Constructors @this)
		=> @this.WhichAre(AccessModifiers.Protected);

	/// <summary>
	///     Filters for constructors that are not protected.
	/// </summary>
	public static Filtered.Constructors WhichAreNotProtected(this Filtered.Constructors @this)
		=> @this.WhichAreNot(AccessModifiers.Protected);
}
