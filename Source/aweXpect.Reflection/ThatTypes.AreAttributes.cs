using System;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatTypes
{
	[GenerateCollectionExpectation("AreAttributes", "is an attribute", "are attributes",
		CollectionOverload = true, NegatedName = "AreNotAttributes",
		NullItemFailsBothWays = true)]
	private static PredicateResult IsAnAttributeType(Type type)
		=> type.IsAttribute();
}
