using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatProperties
{
	[GenerateCollectionExpectation("AreExtensionProperties", "is an extension property", "are extension properties",
		CollectionOverload = true, NegatedName = "AreNotExtensionProperties", FormattedItem = nameof(Formattable),
		NullItemFailsBothWays = true)]
	private static PredicateResult IsAnExtensionPropertyInfo(PropertyInfo property)
		=> property.IsExtensionProperty();
}
