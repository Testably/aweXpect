using System;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatTypes
{
	[GenerateCollectionExpectation("AreStructs", "is a struct", "are structs",
		CollectionOverload = true, NegatedName = "AreNotStructs",
		NullItemFailsBothWays = true)]
	private static PredicateResult IsAStructType(Type type)
		=> type.IsReallyStruct();
}
