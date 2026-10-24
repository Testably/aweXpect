using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatEvent
{
	[GenerateExpectation("Is{Not}Nullable", "is {not} nullable",
		Result = nameof(DescribeNullableEvent),
		Summary = "Verifies that the <see cref=\"EventInfo\" /> is nullable.",
		NegatedSummary = "Verifies that the <see cref=\"EventInfo\" /> is not nullable.",
		Remarks = """
		          An event is considered nullable if its handler type is annotated as nullable
		          (according to the nullable reference type metadata).
		          """,
		NegatedRemarks = """
		                 An event is considered nullable if its handler type is annotated as nullable
		                 (according to the nullable reference type metadata).
		                 Events without nullability annotations (oblivious code) count as non-nullable.
		                 """)]
	private static bool IsNullableEvent(EventInfo @event) => @event.IsNullable();

	private static string DescribeNullableEvent(EventInfo @event)
		=> ResultTexts.Describe(@event.IsNullable(), "was nullable", "was non-nullable", @event);
}
