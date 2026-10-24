using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatEvents
{
	[GenerateCollectionExpectation("AreAbstract", "is abstract", "are abstract",
		CollectionOverload = true, NegatedName = "AreNotAbstract", FormattedItem = nameof(Formattable))]
	private static PredicateResult IsAbstractEvent(EventInfo? @event)
		=> @event is null ? ItemResults.Null : @event.IsReallyAbstract();
}
