using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatEvents
{
	[GenerateCollectionExpectation("AreVirtual", "is virtual", "are virtual",
		CollectionOverload = true, NegatedName = "AreNotVirtual", FormattedItem = nameof(Formattable),
		NullItemFailsBothWays = true)]
	private static PredicateResult IsVirtualEvent(EventInfo @event)
		=> @event.IsReallyVirtual();
}
