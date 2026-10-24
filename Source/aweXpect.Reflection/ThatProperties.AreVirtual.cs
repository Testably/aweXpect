using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatProperties
{
	[GenerateCollectionExpectation("AreVirtual", "is virtual", "are virtual",
		OnCollection = true, NegatedName = "AreNotVirtual", FormattedItem = nameof(Formattable))]
	private static PredicateResult IsVirtualProperty(PropertyInfo? property)
		=> property is null ? ItemResults.Null : property.IsReallyVirtual();
}
