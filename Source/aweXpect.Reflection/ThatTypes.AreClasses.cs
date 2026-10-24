using System;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatTypes
{
	[GenerateCollectionExpectation("AreClasses", "is a class", "are classes",
		CollectionOverload = true, NegatedName = "AreNotClasses")]
	private static PredicateResult IsAClassType(Type? type)
		=> type is null ? ItemResults.Null : type.IsReallyClass();
}
