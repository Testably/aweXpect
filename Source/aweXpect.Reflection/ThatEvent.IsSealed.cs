using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatEvent
{
	[GenerateExpectation("Is{Not}Sealed", "is {not} sealed",
		Result = nameof(DescribeSealedEvent),
		Summary = "Verifies that the <see cref=\"EventInfo\" /> is sealed.",
		NegatedSummary = "Verifies that the <see cref=\"EventInfo\" /> is not sealed.")]
	private static bool IsSealedEvent(EventInfo @event) => @event.IsReallySealed();

	private static string DescribeSealedEvent(EventInfo @event)
		=> ResultTexts.Describe(@event.IsReallySealed(), "was sealed", "was non-sealed", @event);
}
