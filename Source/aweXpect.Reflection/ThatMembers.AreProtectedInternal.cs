using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatMembers
{
	[GenerateCollectionExpectation("AreProtectedInternal", "is protected internal", "are protected internal",
		CollectionOverload = true, NegatedName = "AreNotProtectedInternal", FormattedItem = nameof(Formattable),
		NullItemFailsBothWays = true)]
	private static PredicateResult IsProtectedInternalMember<TMember>(TMember member)
		where TMember : MemberInfo
		=> member.HasAccessModifier(AccessModifiers.ProtectedInternal);
}
