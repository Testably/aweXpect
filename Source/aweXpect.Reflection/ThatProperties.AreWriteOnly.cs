using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatProperties
{
	[GenerateCollectionExpectation("AreWriteOnly", "is write-only", "are write-only",
		CollectionOverload = true, FormattedItem = nameof(Formattable),
		NullItemFailsBothWays = true)]
	private static PredicateResult IsWriteOnlyProperty(PropertyInfo property)
		=> property.IsWriteOnly();
}
