using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatProperties
{
	[GenerateCollectionExpectation("AreAbstract", "is abstract", "are abstract",
		OnCollection = true, NegatedName = "AreNotAbstract", FormattedItem = nameof(Formattable))]
	private static PredicateResult IsAbstractProperty(PropertyInfo? property)
		=> property is null ? ItemResults.Null : property.IsReallyAbstract();
}
