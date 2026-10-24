using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatProperties
{
	[GenerateCollectionExpectation("AreNullable", "is nullable", "are nullable",
		CollectionOverload = true, NegatedName = "AreNotNullable", FormattedItem = nameof(Formattable))]
	private static PredicateResult IsNullableProperty(PropertyInfo? property)
		=> property is null ? ItemResults.Null : property.IsNullable();
}
