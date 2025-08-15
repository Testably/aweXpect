using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class ConstructorFilters
{
	/// <summary>
	///     Filters for constructors that are private.
	/// </summary>
	public static Filtered.Constructors WhichArePrivate(this Filtered.Constructors @this)
		=> @this.WhichAre(AccessModifiers.Private);

	/// <summary>
	///     Filters for constructors that are not private.
	/// </summary>
	public static Filtered.Constructors WhichAreNotPrivate(this Filtered.Constructors @this)
		=> @this.WhichAreNot(AccessModifiers.Private);
}
