using System;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatTypes
{
	[GenerateCollectionExpectation("AreStatic", "is static", "are static",
		OnCollection = true, NegatedName = "AreNotStatic")]
	private static PredicateResult IsStaticType(Type? type)
		=> type is null ? ItemResults.Null : type.IsReallyStatic();
}
