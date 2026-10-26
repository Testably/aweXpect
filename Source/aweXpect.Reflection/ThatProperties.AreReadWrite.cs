using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatProperties
{
	[GenerateCollectionExpectation("AreReadWrite", "is read-write", "are read-write",
		CollectionOverload = true, FormattedItem = nameof(Formattable),
		NullItemFailsBothWays = true)]
	private static PredicateResult IsReadWriteProperty(PropertyInfo property)
		=> property.IsReadWrite();
}
