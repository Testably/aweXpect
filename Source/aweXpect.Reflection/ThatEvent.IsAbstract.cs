using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatEvent
{
	[GenerateExpectation("Is{Not}Abstract", "is {not} abstract",
		Result = nameof(DescribeAbstractEvent),
		Summary = "Verifies that the <see cref=\"EventInfo\" /> is abstract.",
		NegatedSummary = "Verifies that the <see cref=\"EventInfo\" /> is not abstract.")]
	private static bool IsAbstractEvent(EventInfo @event) => @event.IsReallyAbstract();

	private static string DescribeAbstractEvent(EventInfo @event)
		=> ResultTexts.Describe(@event.IsReallyAbstract(), "was abstract", "was non-abstract", @event);
}
