using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatMembers
{
	[GenerateCollectionExpectation("ArePrivate", "is private", "are private",
		CollectionOverload = true, NegatedName = "AreNotPrivate", FormattedItem = nameof(Formattable))]
	private static PredicateResult IsPrivateMember<TMember>(TMember? member)
		where TMember : MemberInfo
		=> member is null ? ItemResults.Null : member.HasAccessModifier(AccessModifiers.Private);
}
