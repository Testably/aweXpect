using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatMembers
{
	[GenerateCollectionExpectation("AreObsolete", "is obsolete", "are obsolete",
		OnCollection = true, NegatedName = "AreNotObsolete", FormattedItem = nameof(Formattable))]
	private static PredicateResult IsObsoleteMember<TMember>(TMember? member)
		where TMember : MemberInfo
		=> member is null ? ItemResults.Null : member.IsObsolete();
}
