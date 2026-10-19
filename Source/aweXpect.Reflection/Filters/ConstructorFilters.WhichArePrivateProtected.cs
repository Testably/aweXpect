using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class ConstructorFilters
{
	/// <summary>
	///     Filters for constructors that are private protected.
	/// </summary>
	public static Filtered.Constructors WhichArePrivateProtected(this Filtered.Constructors @this)
		=> @this.WhichAre(AccessModifiers.PrivateProtected);

	/// <summary>
	///     Filters for constructors that are not private protected.
	/// </summary>
	public static Filtered.Constructors WhichAreNotPrivateProtected(this Filtered.Constructors @this)
		=> @this.WhichAreNot(AccessModifiers.PrivateProtected);
}
