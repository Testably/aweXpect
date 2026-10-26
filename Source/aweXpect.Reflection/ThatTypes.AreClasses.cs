using System;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatTypes
{
	[GenerateCollectionExpectation("AreClasses", "is a class", "are classes",
		CollectionOverload = true, NegatedName = "AreNotClasses",
		NullItemFailsBothWays = true)]
	private static PredicateResult IsAClassType(Type type)
		=> type.IsReallyClass();
}
