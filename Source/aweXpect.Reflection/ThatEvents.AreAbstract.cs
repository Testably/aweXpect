using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatEvents
{
	[GenerateCollectionExpectation("AreAbstract", "is abstract", "are abstract",
		CollectionOverload = true, NegatedName = "AreNotAbstract", FormattedItem = nameof(Formattable),
		NullItemFailsBothWays = true)]
	private static PredicateResult IsAbstractEvent(EventInfo @event)
		=> @event.IsReallyAbstract();
}
