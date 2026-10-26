using System;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatTypes
{
	[GenerateCollectionExpectation("AreRefStructs", "is a ref struct", "are ref structs",
		CollectionOverload = true, NegatedName = "AreNotRefStructs",
		NullItemFailsBothWays = true)]
	private static PredicateResult IsARefStructType(Type type)
		=> type.IsRefStruct();
}
