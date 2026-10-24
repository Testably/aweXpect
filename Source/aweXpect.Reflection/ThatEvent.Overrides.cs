using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatEvent
{
	[GenerateExpectation("Overrides", "overrides a base event",
		NegatedName = "DoesNotOverride",
		NegatedExpectationText = "does not override a base event",
		Result = nameof(DescribeOverridingEvent),
		Summary = "Verifies that the <see cref=\"EventInfo\" /> overrides a base class event.",
		NegatedSummary = "Verifies that the <see cref=\"EventInfo\" /> does not override a base class event.")]
	private static bool OverridesEvent(EventInfo @event) => @event.IsOverride();

	private static string DescribeOverridingEvent(EventInfo @event)
		=> ResultTexts.Describe(@event.IsOverride(),
			"did override a base event", "did not override a base event", @event);
}
