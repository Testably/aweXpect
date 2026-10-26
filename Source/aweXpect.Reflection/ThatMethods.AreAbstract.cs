using System.Reflection;
using aweXpect.Core.Extending;

namespace aweXpect;

public static partial class ThatMethods
{
	[GenerateCollectionExpectation("AreAbstract", "is abstract", "are abstract",
		CollectionOverload = true, NegatedName = "AreNotAbstract", FormattedItem = nameof(Formattable),
		NullItemFailsBothWays = true)]
	private static PredicateResult IsAbstractMethod(MethodInfo method)
		=> method.IsAbstract;
}
