using System;
using aweXpect.Core.Extending;

namespace aweXpect;

public static partial class ThatTypes
{
	[GenerateCollectionExpectation("AreInterfaces", "is an interface", "are interfaces",
		CollectionOverload = true, NegatedName = "AreNotInterfaces",
		NullItemFailsBothWays = true)]
	private static PredicateResult IsAnInterfaceType(Type type)
		=> type.IsInterface;
}
