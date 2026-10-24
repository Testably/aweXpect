using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatEvents
{
	[GenerateCollectionExpectation("AreVirtual", "is virtual", "are virtual",
		OnCollection = true, NegatedName = "AreNotVirtual", FormattedItem = nameof(Formattable))]
	private static PredicateResult IsVirtualEvent(EventInfo? @event)
		=> @event is null ? ItemResults.Null : @event.IsReallyVirtual();
}
