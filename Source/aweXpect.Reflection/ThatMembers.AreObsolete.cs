using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatMembers
{
	[GenerateCollectionExpectation("AreObsolete", "is obsolete", "are obsolete",
		CollectionOverload = true, NegatedName = "AreNotObsolete", FormattedItem = nameof(Formattable),
		NullItemFailsBothWays = true)]
	private static PredicateResult IsObsoleteMember<TMember>(TMember member)
		where TMember : MemberInfo
		=> member.IsObsolete();
}
