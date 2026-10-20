using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class FieldFilters
{
	/// <summary>
	///     Filters for fields that are internal.
	/// </summary>
	public static Filtered.Fields WhichAreInternal(this Filtered.Fields @this)
		=> @this.WhichAre(AccessModifiers.Internal);

	/// <summary>
	///     Filters for fields that are not internal.
	/// </summary>
	public static Filtered.Fields WhichAreNotInternal(this Filtered.Fields @this)
		=> @this.WhichAreNot(AccessModifiers.Internal);
}
