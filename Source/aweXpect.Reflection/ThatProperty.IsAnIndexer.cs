using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatProperty
{
	[GenerateExpectation("Is{Not}AnIndexer", "is {not} an indexer",
		Result = nameof(DescribeIndexer),
		Summary = "Verifies that the <see cref=\"PropertyInfo\" /> is an indexer (has index parameters).",
		NegatedSummary = "Verifies that the <see cref=\"PropertyInfo\" /> is not an indexer (has no index parameters).")]
	private static bool IsAnIndexerProperty(PropertyInfo property) => property.IsIndexer();

	private static string DescribeIndexer(PropertyInfo property)
		=> ResultTexts.Describe(property.IsIndexer(), "was an indexer", "was not an indexer", property);
}
