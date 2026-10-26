using System.Reflection;
using aweXpect.Core.Extending;

namespace aweXpect;

public static partial class ThatMethods
{
	[GenerateCollectionExpectation("AreVirtual", "is virtual", "are virtual",
		CollectionOverload = true, NegatedName = "AreNotVirtual", FormattedItem = nameof(Formattable),
		NullItemFailsBothWays = true)]
	private static PredicateResult IsVirtualMethod(MethodInfo method)
		=> method.IsVirtual;
}
