using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatMethods
{
	[GenerateCollectionExpectation("AreOperators", "is an operator", "are operators",
		OnCollection = true, NegatedName = "AreNotOperators", FormattedItem = nameof(Formattable))]
	private static PredicateResult IsAnOperatorMethod(MethodInfo? method)
		=> method is null ? ItemResults.Null : method.IsOperator();
}
