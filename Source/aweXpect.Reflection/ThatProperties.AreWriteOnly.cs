using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatProperties
{
	[GenerateCollectionExpectation("AreWriteOnly", "is write-only", "are write-only",
		OnCollection = true, FormattedItem = nameof(Formattable))]
	private static PredicateResult IsWriteOnlyProperty(PropertyInfo? property)
		=> property is null ? ItemResults.Null : property.IsWriteOnly();
}
