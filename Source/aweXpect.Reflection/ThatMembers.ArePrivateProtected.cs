using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatMembers
{
	[GenerateCollectionExpectation("ArePrivateProtected", "is private protected", "are private protected",
		CollectionOverload = true, NegatedName = "AreNotPrivateProtected", FormattedItem = nameof(Formattable),
		NullItemFailsBothWays = true)]
	private static PredicateResult IsPrivateProtectedMember<TMember>(TMember member)
		where TMember : MemberInfo
		=> member.HasAccessModifier(AccessModifiers.PrivateProtected);
}
