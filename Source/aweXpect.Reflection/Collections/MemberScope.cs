namespace aweXpect.Reflection.Collections;

/// <summary>
///     Specifies which members are included when navigating from types to their members.
/// </summary>
public enum MemberScope
{
	/// <summary>
	///     Only include members declared directly on the type, excluding inherited members.
	/// </summary>
	DeclaredOnly,

	/// <summary>
	///     Include members declared directly on the type as well as members inherited from base types.
	/// </summary>
	IncludingInherited,
}
