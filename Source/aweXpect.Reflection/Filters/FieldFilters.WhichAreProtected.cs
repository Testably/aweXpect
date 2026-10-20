using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class FieldFilters
{
	/// <summary>
	///     Filters for fields that are protected.
	/// </summary>
	public static Filtered.Fields WhichAreProtected(this Filtered.Fields @this)
		=> @this.WhichAre(AccessModifiers.Protected);

	/// <summary>
	///     Filters for fields that are not protected.
	/// </summary>
	public static Filtered.Fields WhichAreNotProtected(this Filtered.Fields @this)
		=> @this.WhichAreNot(AccessModifiers.Protected);
}
