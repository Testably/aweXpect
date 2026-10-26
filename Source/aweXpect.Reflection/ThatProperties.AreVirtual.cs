using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatProperties
{
	[GenerateCollectionExpectation("AreVirtual", "is virtual", "are virtual",
		CollectionOverload = true, NegatedName = "AreNotVirtual", FormattedItem = nameof(Formattable),
		NullItemFailsBothWays = true)]
	private static PredicateResult IsVirtualProperty(PropertyInfo property)
		=> property.IsReallyVirtual();
}
