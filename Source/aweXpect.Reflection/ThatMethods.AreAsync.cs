using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatMethods
{
	[GenerateCollectionExpectation("AreAsync", "is async", "are async",
		CollectionOverload = true, NegatedName = "AreNotAsync", FormattedItem = nameof(Formattable))]
	private static PredicateResult IsAsyncMethod(MethodInfo? method)
		=> method is null ? ItemResults.Null : method.IsAsync();
}
