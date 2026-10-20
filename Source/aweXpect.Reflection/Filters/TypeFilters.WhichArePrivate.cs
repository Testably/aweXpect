using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class TypeFilters
{
	/// <summary>
	///     Filters for types that are private.
	/// </summary>
	public static Filtered.Types WhichArePrivate(this Filtered.Types @this)
		=> @this.WhichAre(AccessModifiers.Private);

	/// <summary>
	///     Filters for types that are not private.
	/// </summary>
	public static Filtered.Types WhichAreNotPrivate(this Filtered.Types @this)
		=> @this.WhichAreNot(AccessModifiers.Private);
}
