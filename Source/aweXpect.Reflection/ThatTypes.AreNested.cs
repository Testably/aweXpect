using System;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatTypes
{
	[GenerateCollectionExpectation("AreNested", "is nested", "are nested",
		CollectionOverload = true, NegatedName = "AreNotNested")]
	private static PredicateResult IsNestedType(Type? type)
		=> type is null ? ItemResults.Null : type.IsNested;
}
