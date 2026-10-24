using System;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatTypes
{
	[GenerateCollectionExpectation("AreRecordStructs", "is a record struct", "are record structs",
		OnCollection = true, NegatedName = "AreNotRecordStructs")]
	private static PredicateResult IsARecordStructType(Type? type)
		=> type is null ? ItemResults.Null : type.IsRecordStruct();
}
