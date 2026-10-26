using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatAssemblies
{
	[GenerateCollectionExpectation("AreStrongNamed", "is strong named", "are strong named",
		CollectionOverload = true, NegatedName = "AreNotStrongNamed",
		NullItemFailsBothWays = true)]
	private static PredicateResult IsStrongNamedAssembly(Assembly assembly)
		=> assembly.IsStrongNamed();
}
