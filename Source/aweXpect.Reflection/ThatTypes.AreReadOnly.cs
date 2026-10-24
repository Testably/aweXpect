using System;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatTypes
{
	[GenerateCollectionExpectation("AreReadOnly", "is read-only", "are read-only",
		OnCollection = true, NegatedName = "AreNotReadOnly")]
	private static PredicateResult IsReadOnlyType(Type? type)
		=> type is null ? ItemResults.Null : type.IsReadOnlyStruct();
}
