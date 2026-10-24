using System;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatTypes
{
	[GenerateCollectionExpectation("AreRecords", "is a record", "are records",
		OnCollection = true, NegatedName = "AreNotRecords")]
	private static PredicateResult IsARecordType(Type? type)
		=> type is null ? ItemResults.Null : type.IsRecordClass();
}
