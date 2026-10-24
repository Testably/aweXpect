using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatMembers
{
	[GenerateCollectionExpectation("AreInternal", "is internal", "are internal",
		CollectionOverload = true, NegatedName = "AreNotInternal", FormattedItem = nameof(Formattable))]
	private static PredicateResult IsInternalMember<TMember>(TMember? member)
		where TMember : MemberInfo
		=> member is null ? ItemResults.Null : member.HasAccessModifier(AccessModifiers.Internal);
}
