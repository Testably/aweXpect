using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatProperties
{
	[GenerateCollectionExpectation("AreRequired", "is required", "are required",
		CollectionOverload = true, NegatedName = "AreNotRequired", FormattedItem = nameof(Formattable),
		NullItemFailsBothWays = true)]
	private static PredicateResult IsRequiredProperty(PropertyInfo property)
		=> property.IsRequired();
}
