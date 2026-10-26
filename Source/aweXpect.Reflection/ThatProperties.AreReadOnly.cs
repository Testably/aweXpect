using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatProperties
{
	[GenerateCollectionExpectation("AreReadOnly", "is read-only", "are read-only",
		CollectionOverload = true, FormattedItem = nameof(Formattable),
		NullItemFailsBothWays = true)]
	private static PredicateResult IsReadOnlyProperty(PropertyInfo property)
		=> property.IsReadOnly();
}
