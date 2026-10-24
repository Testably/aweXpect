using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatEvents
{
	[GenerateCollectionExpectation("AreStatic", "is static", "are static",
		OnCollection = true, NegatedName = "AreNotStatic", FormattedItem = nameof(Formattable))]
	private static PredicateResult IsStaticEvent(EventInfo? @event)
		=> @event is null ? ItemResults.Null : @event.IsReallyStatic();
}
