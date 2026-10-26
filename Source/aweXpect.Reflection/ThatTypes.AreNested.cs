using System;
using aweXpect.Core.Extending;

namespace aweXpect;

public static partial class ThatTypes
{
	[GenerateCollectionExpectation("AreNested", "is nested", "are nested",
		CollectionOverload = true, NegatedName = "AreNotNested",
		NullItemFailsBothWays = true)]
	private static PredicateResult IsNestedType(Type type)
		=> type.IsNested;
}
