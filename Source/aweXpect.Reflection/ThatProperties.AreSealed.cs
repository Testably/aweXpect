using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatProperties
{
	[GenerateCollectionExpectation("AreSealed", "is sealed", "are sealed",
		CollectionOverload = true, NegatedName = "AreNotSealed", FormattedItem = nameof(Formattable),
		NullItemFailsBothWays = true)]
	private static PredicateResult IsSealedProperty(PropertyInfo property)
		=> property.IsReallySealed();
}
