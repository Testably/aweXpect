using System;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatTypes
{
	[GenerateCollectionExpectation("AreRecordStructs", "is a record struct", "are record structs",
		CollectionOverload = true, NegatedName = "AreNotRecordStructs",
		NullItemFailsBothWays = true)]
	private static PredicateResult IsARecordStructType(Type type)
		=> type.IsRecordStruct();
}
