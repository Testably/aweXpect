using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatMethods
{
	[GenerateCollectionExpectation("AreAbstract", "is abstract", "are abstract",
		CollectionOverload = true, NegatedName = "AreNotAbstract", FormattedItem = nameof(Formattable))]
	private static PredicateResult IsAbstractMethod(MethodInfo? method)
		=> method is null ? ItemResults.Null : method.IsAbstract;
}
