using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatMembers
{
	[GenerateCollectionExpectation("AreProtected", "is protected", "are protected",
		CollectionOverload = true, NegatedName = "AreNotProtected", FormattedItem = nameof(Formattable))]
	private static PredicateResult IsProtectedMember<TMember>(TMember? member)
		where TMember : MemberInfo
		=> member is null ? ItemResults.Null : member.HasAccessModifier(AccessModifiers.Protected);
}
