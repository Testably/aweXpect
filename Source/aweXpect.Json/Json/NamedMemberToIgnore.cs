using aweXpect.Equivalency;

namespace aweXpect.Json;

/// <summary>
///     A <see cref="MemberToIgnore.ByName" /> that keeps its name, which the referenced aweXpect.Core does not expose, to
///     name it when it matched no member.
/// </summary>
internal sealed class NamedMemberToIgnore : MemberToIgnore.ByName
{
	public NamedMemberToIgnore(string memberName) : base(memberName)
	{
		MemberName = memberName;
	}

	public string MemberName { get; }
}
