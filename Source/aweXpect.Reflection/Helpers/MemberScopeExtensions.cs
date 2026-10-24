using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Helpers;

/// <summary>
///     Extension methods on <see cref="MemberScope" />.
/// </summary>
internal static class MemberScopeExtensions
{
	/// <summary>
	///     Checks if the <paramref name="memberScope" /> only considers declared elements or direct relations.
	/// </summary>
	public static bool IsDirect(this MemberScope memberScope)
		=> (memberScope & MemberScope.DeclaredOnly) != 0;

	/// <summary>
	///     Checks if the <paramref name="memberScope" /> includes the members declared on <see cref="object" />,
	///     <see cref="System.ValueType" /> and <see cref="System.Enum" />.
	/// </summary>
	public static bool IncludesObjectMembers(this MemberScope memberScope)
		=> !memberScope.IsDirect() && (memberScope & MemberScope.IncludingObjectMembers) != 0;

	/// <summary>
	///     The text that a check appends to its members when it includes the members declared on <see cref="object" />,
	///     <see cref="System.ValueType" /> and <see cref="System.Enum" />, otherwise an empty string.
	/// </summary>
	public static string ObjectMembersText(this MemberScope memberScope)
		=> memberScope.IncludesObjectMembers() ? "including object members " : "";

	/// <summary>
	///     Describes the members of a listing in the <paramref name="memberScope" />, e.g. <c>"methods "</c> for the
	///     declared methods or <c>"declared and inherited methods "</c>.
	/// </summary>
	public static string DescribeListing(this MemberScope memberScope, string description)
	{
		if (memberScope.IsDirect())
		{
			return description;
		}

		return memberScope.IncludesObjectMembers()
			? $"declared and inherited {description}including object members "
			: $"declared and inherited {description}";
	}
}
