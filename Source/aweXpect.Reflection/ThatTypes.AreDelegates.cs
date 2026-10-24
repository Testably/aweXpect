using System;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatTypes
{
	[GenerateCollectionExpectation("AreDelegates", "is a delegate", "are delegates",
		CollectionOverload = true, NegatedName = "AreNotDelegates")]
	private static PredicateResult IsADelegateType(Type? type)
		=> type is null ? ItemResults.Null : type.IsDelegate();
}
