using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatProperties
{
	[GenerateCollectionExpectation("Override", "overrides a base property", "override a base property",
		CollectionOverload = true, NegatedName = "DoNotOverride", FormattedItem = nameof(Formattable))]
	private static PredicateResult OverridesProperty(PropertyInfo? property)
		=> property is null ? ItemResults.Null : property.IsOverride();
}
