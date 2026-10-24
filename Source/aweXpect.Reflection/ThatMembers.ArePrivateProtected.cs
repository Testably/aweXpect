using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatMembers
{
	[GenerateCollectionExpectation("ArePrivateProtected", "is private protected", "are private protected",
		CollectionOverload = true, NegatedName = "AreNotPrivateProtected", FormattedItem = nameof(Formattable))]
	private static PredicateResult IsPrivateProtectedMember<TMember>(TMember? member)
		where TMember : MemberInfo
		=> member is null ? ItemResults.Null : member.HasAccessModifier(AccessModifiers.PrivateProtected);
}
