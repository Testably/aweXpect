using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatProperties
{
	[GenerateCollectionExpectation("AreWritable", "is writable", "are writable",
		CollectionOverload = true, FormattedItem = nameof(Formattable),
		NullItemFailsBothWays = true)]
	private static PredicateResult IsWritableProperty(PropertyInfo property)
		=> property.IsWritable();
}
