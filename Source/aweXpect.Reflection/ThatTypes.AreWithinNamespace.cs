using System;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatTypes
{
	[GenerateCollectionExpectation("AreWithinNamespace", "is within namespace {expected}",
		"are within namespace {expected}",
		CollectionOverload = true, NegatedName = "AreNotWithinNamespace",
		Remarks = "Sub-namespaces of the <paramref name=\"expected\" /> namespace are included.")]
	private static PredicateResult IsWithinNamespaceType(Type? type, string expected)
		=> type is null ? ItemResults.Null : type.IsWithinNamespace(expected);
}
