using System;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatTypes
{
	[GenerateCollectionExpectation("HaveADefaultConstructor", "has a default constructor", "have a default constructor",
		OnCollection = true, NegatedName = "DoNotHaveADefaultConstructor")]
	private static PredicateResult HasADefaultConstructorType(Type? type)
		=> type is null ? ItemResults.Null : type.HasDefaultConstructor();
}
