using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatProperties
{
	[GenerateCollectionExpectation("AreRequired", "is required", "are required",
		CollectionOverload = true, NegatedName = "AreNotRequired", FormattedItem = nameof(Formattable))]
	private static PredicateResult IsRequiredProperty(PropertyInfo? property)
		=> property is null ? ItemResults.Null : property.IsRequired();
}
