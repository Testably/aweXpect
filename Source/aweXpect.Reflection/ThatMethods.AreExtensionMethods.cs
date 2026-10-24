using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatMethods
{
	[GenerateCollectionExpectation("AreExtensionMethods", "is an extension method", "are extension methods",
		CollectionOverload = true, NegatedName = "AreNotExtensionMethods", FormattedItem = nameof(Formattable))]
	private static PredicateResult IsAnExtensionMethodInfo(MethodInfo? method)
		=> method is null ? ItemResults.Null : method.IsExtensionMethod();
}
