using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class FieldFilters
{
	/// <summary>
	///     Filters for fields that are protected internal.
	/// </summary>
	public static Filtered.Fields WhichAreProtectedInternal(this Filtered.Fields @this)
		=> @this.WhichAre(AccessModifiers.ProtectedInternal);

	/// <summary>
	///     Filters for fields that are not protected internal.
	/// </summary>
	public static Filtered.Fields WhichAreNotProtectedInternal(this Filtered.Fields @this)
		=> @this.WhichAreNot(AccessModifiers.ProtectedInternal);
}
