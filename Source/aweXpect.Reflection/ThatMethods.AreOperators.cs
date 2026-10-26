using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatMethods
{
	[GenerateCollectionExpectation("AreOperators", "is an operator", "are operators",
		CollectionOverload = true, NegatedName = "AreNotOperators", FormattedItem = nameof(Formattable),
		NullItemFailsBothWays = true)]
	private static PredicateResult IsAnOperatorMethod(MethodInfo method)
		=> method.IsOperator();

	[GenerateCollectionExpectation("AreOperators", "is the operator {operator}", "are the operator {operator}",
		CollectionOverload = true, NegatedName = "AreNotOperators", FormattedItem = nameof(Formattable),
		NullItemFailsBothWays = true)]
	private static PredicateResult IsTheOperatorMethod(MethodInfo method, Operator @operator)
		=> method.IsOperator(@operator);
}
