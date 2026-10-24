using System;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatTypes
{
	[GenerateCollectionExpectation("AreEnums", "is an enum", "are enums",
		OnCollection = true, NegatedName = "AreNotEnums")]
	private static PredicateResult IsAnEnumType(Type? type)
		=> type is null ? ItemResults.Null : type.IsEnum;
}
