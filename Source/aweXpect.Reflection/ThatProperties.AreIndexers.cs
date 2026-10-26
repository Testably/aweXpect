using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatProperties
{
	[GenerateCollectionExpectation("AreIndexers", "is an indexer", "are indexers",
		CollectionOverload = true, NegatedName = "AreNotIndexers", FormattedItem = nameof(Formattable),
		NullItemFailsBothWays = true)]
	private static PredicateResult IsAnIndexerProperty(PropertyInfo property)
		=> property.IsIndexer();
}
