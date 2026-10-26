using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatMethods
{
	[GenerateCollectionExpectation("Override", "overrides a base method", "override a base method",
		CollectionOverload = true, NegatedName = "DoNotOverride", FormattedItem = nameof(Formattable),
		NullItemFailsBothWays = true)]
	private static PredicateResult OverridesMethod(MethodInfo method)
		=> method.IsOverride();
}
