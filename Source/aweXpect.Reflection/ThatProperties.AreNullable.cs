using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatProperties
{
	[GenerateCollectionExpectation("AreNullable", "is nullable", "are nullable",
		CollectionOverload = true, NegatedName = "AreNotNullable", FormattedItem = nameof(Formattable),
		NullItemFailsBothWays = true)]
	private static PredicateResult IsNullableProperty(PropertyInfo property)
		=> property.IsNullable();
}
