using System;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatTypes
{
	[GenerateCollectionExpectation("AreRecords", "is a record", "are records",
		CollectionOverload = true, NegatedName = "AreNotRecords",
		NullItemFailsBothWays = true)]
	private static PredicateResult IsARecordType(Type type)
		=> type.IsRecordClass();
}
