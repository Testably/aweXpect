using System;
using aweXpect.Core.Extending;

namespace aweXpect;

public static partial class ThatTypes
{
	[GenerateCollectionExpectation("AreEnums", "is an enum", "are enums",
		CollectionOverload = true, NegatedName = "AreNotEnums",
		NullItemFailsBothWays = true)]
	private static PredicateResult IsAnEnumType(Type type)
		=> type.IsEnum;
}
