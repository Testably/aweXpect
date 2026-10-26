using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatProperties
{
	[GenerateCollectionExpectation("AreReadable", "is readable", "are readable",
		CollectionOverload = true, FormattedItem = nameof(Formattable),
		NullItemFailsBothWays = true)]
	private static PredicateResult IsReadableProperty(PropertyInfo property)
		=> property.IsReadable();
}
