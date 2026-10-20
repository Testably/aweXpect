using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class ConstructorFilters
{
	/// <summary>
	///     Filters for constructors that are public.
	/// </summary>
	public static Filtered.Constructors WhichArePublic(this Filtered.Constructors @this)
		=> @this.WhichAre(AccessModifiers.Public);

	/// <summary>
	///     Filters for constructors that are not public.
	/// </summary>
	public static Filtered.Constructors WhichAreNotPublic(this Filtered.Constructors @this)
		=> @this.WhichAreNot(AccessModifiers.Public);
}
