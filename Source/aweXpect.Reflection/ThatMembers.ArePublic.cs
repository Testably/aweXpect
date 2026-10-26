using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatMembers
{
	[GenerateCollectionExpectation("ArePublic", "is public", "are public",
		CollectionOverload = true, NegatedName = "AreNotPublic", FormattedItem = nameof(Formattable),
		NullItemFailsBothWays = true)]
	private static PredicateResult IsPublicMember<TMember>(TMember member)
		where TMember : MemberInfo
		=> member.HasAccessModifier(AccessModifiers.Public);
}
