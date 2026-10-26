using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatProperties
{
	[GenerateCollectionExpectation("AreAbstract", "is abstract", "are abstract",
		CollectionOverload = true, NegatedName = "AreNotAbstract", FormattedItem = nameof(Formattable),
		NullItemFailsBothWays = true)]
	private static PredicateResult IsAbstractProperty(PropertyInfo property)
		=> property.IsReallyAbstract();
}
