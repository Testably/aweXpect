using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatEvents
{
	[GenerateCollectionExpectation("AreSealed", "is sealed", "are sealed",
		CollectionOverload = true, NegatedName = "AreNotSealed", FormattedItem = nameof(Formattable),
		NullItemFailsBothWays = true)]
	private static PredicateResult IsSealedEvent(EventInfo @event)
		=> @event.IsReallySealed();
}
