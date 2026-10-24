using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatMethods
{
	[GenerateCollectionExpectation("AreSealed", "is sealed", "are sealed",
		OnCollection = true, NegatedName = "AreNotSealed", FormattedItem = nameof(Formattable))]
	private static PredicateResult IsSealedMethod(MethodInfo? method)
		=> method is null ? ItemResults.Null : method.IsReallySealed();
}
