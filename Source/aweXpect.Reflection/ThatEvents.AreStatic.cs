using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatEvents
{
	[GenerateCollectionExpectation("AreStatic", "is static", "are static",
		CollectionOverload = true, NegatedName = "AreNotStatic", FormattedItem = nameof(Formattable),
		NullItemFailsBothWays = true)]
	private static PredicateResult IsStaticEvent(EventInfo @event)
		=> @event.IsReallyStatic();
}
