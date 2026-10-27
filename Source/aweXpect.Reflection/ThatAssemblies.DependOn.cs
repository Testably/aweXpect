using System.Reflection;
using System.Threading.Tasks;
using aweXpect.Core.Extending;
using aweXpect.Options;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatAssemblies
{
	[GenerateCollectionExpectation("DependOn", "has dependency on assembly {expected}",
		"have dependency on assembly {expected}", CollectionOverload = true, NegatedName = "DoNotDependOn")]
	private static async ValueTask<bool> DependsOnAssembly(Assembly? assembly,
		[NegatedParameterName("unexpected")] string expected, [ExpectationOptions] StringEqualityOptions options)
		=> assembly is not null && await assembly.GetReferencedAssemblyNames()
			.AnyAsync(dependency => options.AreConsideredEqual(dependency.Name, expected, nameof(expected)));
}
