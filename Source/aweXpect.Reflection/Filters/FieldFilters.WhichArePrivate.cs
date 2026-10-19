using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class FieldFilters
{
	/// <summary>
	///     Filters for fields that are private.
	/// </summary>
	public static Filtered.Fields WhichArePrivate(this Filtered.Fields @this)
		=> @this.WhichAre(AccessModifiers.Private);

	/// <summary>
	///     Filters for fields that are not private.
	/// </summary>
	public static Filtered.Fields WhichAreNotPrivate(this Filtered.Fields @this)
		=> @this.WhichAreNot(AccessModifiers.Private);
}
