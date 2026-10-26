using aweXpect.Core;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Helpers;

/// <summary>
///     Extension methods on <see cref="MemberScopes" />.
/// </summary>
internal static class MemberScopeExtensions
{
	/// <summary>
	///     Checks if the <paramref name="memberScope" /> only considers declared elements or direct relations.
	/// </summary>
	public static bool IsDirect(this MemberScopes memberScope)
		=> (memberScope & MemberScopes.DeclaredOnly) != 0;

	/// <summary>
	///     Checks if the <paramref name="memberScope" /> includes the members declared on <see cref="object" />,
	///     <see cref="System.ValueType" /> and <see cref="System.Enum" />.
	/// </summary>
	public static bool IncludesObjectMembers(this MemberScopes memberScope)
		=> !memberScope.IsDirect() && (memberScope & MemberScopes.IncludingObjectMembers) != 0;

	/// <summary>
	///     The text that a check appends to its members when it includes the members declared on <see cref="object" />,
	///     <see cref="System.ValueType" /> and <see cref="System.Enum" />, otherwise an empty string.
	/// </summary>
	public static string ObjectMembersText(this MemberScopes memberScope)
		=> memberScope.IncludesObjectMembers() ? "including object members " : "";

	/// <summary>
	///     Describes the members of a listing in the <paramref name="memberScope" />, e.g. <c>"methods "</c> for the
	///     declared methods or <c>"declared and inherited methods "</c>.
	/// </summary>
	public static string DescribeListing(this MemberScopes memberScope, string description)
	{
		if (memberScope.IsDirect())
		{
			return description;
		}

		return memberScope.IncludesObjectMembers()
			? $"declared and inherited {description}including object members "
			: $"declared and inherited {description}";
	}

	/// <summary>
	///     Restricts the scope in the <paramref name="holder" /> to declared elements or direct relations.
	/// </summary>
	public static OptionsHolder<MemberScopes> Directly(this OptionsHolder<MemberScopes> holder)
	{
		holder.Value |= MemberScopes.DeclaredOnly;
		return holder;
	}

	/// <summary>
	///     Adds the members declared on <see cref="object" />, <see cref="System.ValueType" /> and
	///     <see cref="System.Enum" /> to the scope in the <paramref name="holder" />.
	/// </summary>
	public static OptionsHolder<MemberScopes> IncludingObjectMembers(this OptionsHolder<MemberScopes> holder)
	{
		holder.Value |= MemberScopes.IncludingObjectMembers;
		return holder;
	}
}
