using System;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatTypes
{
	[GenerateCollectionExpectation("AreInterfaces", "is an interface", "are interfaces",
		CollectionOverload = true, NegatedName = "AreNotInterfaces")]
	private static PredicateResult IsAnInterfaceType(Type? type)
		=> type is null ? ItemResults.Null : type.IsInterface;
}
