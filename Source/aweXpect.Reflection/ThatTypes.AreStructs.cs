using System;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatTypes
{
	[GenerateCollectionExpectation("AreStructs", "is a struct", "are structs",
		CollectionOverload = true, NegatedName = "AreNotStructs")]
	private static PredicateResult IsAStructType(Type? type)
		=> type is null ? ItemResults.Null : type.IsReallyStruct();
}
