using System.Reflection;
using System.Threading.Tasks;
using aweXpect.Core.Extending;
using aweXpect.Options;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatAssemblies
{
	[GenerateCollectionExpectation("Target", "targets {expected}", "target {expected}",
		CollectionOverload = true, NegatedName = "DoNotTarget",
		Summary = "…target the <paramref name=\"expected\" /> framework (e.g. <c>net8.0</c>).")]
	private static async ValueTask<PredicateResult> TargetsFramework(Assembly? assembly,
		[NegatedParameterName("unexpected")] string expected, [ExpectationOptions] StringEqualityOptions options)
		=> assembly is null
			? ItemResults.Null
			: await options.AreConsideredEqual(assembly.GetTargetFramework(), expected, nameof(expected));
}
