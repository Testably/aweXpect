using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatAssemblies
{
	[GenerateCollectionExpectation("AreStrongNamed", "is strong named", "are strong named",
		OnCollection = true, NegatedName = "AreNotStrongNamed")]
	private static PredicateResult IsStrongNamedAssembly(Assembly? assembly)
		=> assembly is null ? ItemResults.Null : assembly.IsStrongNamed();
}
