using System;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatTypes
{
	[GenerateCollectionExpectation("AreRefStructs", "is a ref struct", "are ref structs",
		OnCollection = true, NegatedName = "AreNotRefStructs")]
	private static PredicateResult IsARefStructType(Type? type)
		=> type is null ? ItemResults.Null : type.IsRefStruct();
}
