using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatMembers
{
	[GenerateCollectionExpectation("AreInternal", "is internal", "are internal",
		CollectionOverload = true, NegatedName = "AreNotInternal", FormattedItem = nameof(Formattable),
		NullItemFailsBothWays = true)]
	private static PredicateResult IsInternalMember<TMember>(TMember member)
		where TMember : MemberInfo
		=> member.HasAccessModifier(AccessModifiers.Internal);
}
