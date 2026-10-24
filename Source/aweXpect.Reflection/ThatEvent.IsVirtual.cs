using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatEvent
{
	[GenerateExpectation("Is{Not}Virtual", "is {not} virtual",
		Result = nameof(DescribeVirtualEvent),
		Summary = "Verifies that the <see cref=\"EventInfo\" /> is virtual.",
		NegatedSummary = "Verifies that the <see cref=\"EventInfo\" /> is not virtual.")]
	private static bool IsVirtualEvent(EventInfo @event) => @event.IsReallyVirtual();

	private static string DescribeVirtualEvent(EventInfo @event)
		=> ResultTexts.Describe(@event.IsReallyVirtual(), "was virtual", "was non-virtual", @event);
}
