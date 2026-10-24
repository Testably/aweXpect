using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatMembers
{
	[GenerateCollectionExpectation("ArePublic", "is public", "are public",
		CollectionOverload = true, NegatedName = "AreNotPublic", FormattedItem = nameof(Formattable))]
	private static PredicateResult IsPublicMember<TMember>(TMember? member)
		where TMember : MemberInfo
		=> member is null ? ItemResults.Null : member.HasAccessModifier(AccessModifiers.Public);
}
