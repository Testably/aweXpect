using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatProperties
{
	[GenerateCollectionExpectation("AreReadWrite", "is read-write", "are read-write",
		OnCollection = true, FormattedItem = nameof(Formattable))]
	private static PredicateResult IsReadWriteProperty(PropertyInfo? property)
		=> property is null ? ItemResults.Null : property.IsReadWrite();
}
