using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class ConstructorFilters
{
	/// <summary>
	///     Filters for constructors that are protected internal.
	/// </summary>
	public static Filtered.Constructors WhichAreProtectedInternal(this Filtered.Constructors @this)
		=> @this.WhichAre(AccessModifiers.ProtectedInternal);

	/// <summary>
	///     Filters for constructors that are not protected internal.
	/// </summary>
	public static Filtered.Constructors WhichAreNotProtectedInternal(this Filtered.Constructors @this)
		=> @this.WhichAreNot(AccessModifiers.ProtectedInternal);
}
