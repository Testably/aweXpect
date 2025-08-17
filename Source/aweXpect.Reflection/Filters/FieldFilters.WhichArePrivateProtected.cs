using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class FieldFilters
{
	/// <summary>
	///     Filters for fields that are private protected.
	/// </summary>
	public static Filtered.Fields WhichArePrivateProtected(this Filtered.Fields @this)
		=> @this.WhichAre(AccessModifiers.PrivateProtected);

	/// <summary>
	///     Filters for fields that are not private protected.
	/// </summary>
	public static Filtered.Fields WhichAreNotPrivateProtected(this Filtered.Fields @this)
		=> @this.WhichAreNot(AccessModifiers.PrivateProtected);
}
