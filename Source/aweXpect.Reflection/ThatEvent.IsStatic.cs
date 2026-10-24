using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatEvent
{
	[GenerateExpectation("Is{Not}Static", "is {not} static",
		Result = nameof(DescribeStaticEvent),
		Summary = "Verifies that the <see cref=\"EventInfo\" /> is static.",
		NegatedSummary = "Verifies that the <see cref=\"EventInfo\" /> is not static.")]
	private static bool IsStaticEvent(EventInfo @event) => @event.IsReallyStatic();

	private static string DescribeStaticEvent(EventInfo @event)
		=> ResultTexts.Describe(@event.IsReallyStatic(), "was static", "was non-static", @event);
}
