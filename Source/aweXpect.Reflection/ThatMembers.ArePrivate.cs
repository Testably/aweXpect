using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatMembers
{
	[GenerateCollectionExpectation("ArePrivate", "is private", "are private",
		CollectionOverload = true, NegatedName = "AreNotPrivate", FormattedItem = nameof(Formattable),
		NullItemFailsBothWays = true)]
	private static PredicateResult IsPrivateMember<TMember>(TMember member)
		where TMember : MemberInfo
		=> member.HasAccessModifier(AccessModifiers.Private);
}
