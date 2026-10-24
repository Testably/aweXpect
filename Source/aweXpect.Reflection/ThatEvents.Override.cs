using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatEvents
{
	[GenerateCollectionExpectation("Override", "overrides a base event", "override a base event",
		CollectionOverload = true, NegatedName = "DoNotOverride", FormattedItem = nameof(Formattable))]
	private static PredicateResult OverridesEvent(EventInfo? @event)
		=> @event is null ? ItemResults.Null : @event.IsOverride();
}
