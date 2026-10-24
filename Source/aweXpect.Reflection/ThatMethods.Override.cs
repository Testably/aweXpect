using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatMethods
{
	[GenerateCollectionExpectation("Override", "overrides a base method", "override a base method",
		OnCollection = true, NegatedName = "DoNotOverride", FormattedItem = nameof(Formattable))]
	private static PredicateResult OverridesMethod(MethodInfo? method)
		=> method is null ? ItemResults.Null : method.IsOverride();
}
