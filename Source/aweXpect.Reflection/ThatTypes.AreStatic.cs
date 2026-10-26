using System;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatTypes
{
	[GenerateCollectionExpectation("AreStatic", "is static", "are static",
		CollectionOverload = true, NegatedName = "AreNotStatic",
		NullItemFailsBothWays = true)]
	private static PredicateResult IsStaticType(Type type)
		=> type.IsReallyStatic();
}
