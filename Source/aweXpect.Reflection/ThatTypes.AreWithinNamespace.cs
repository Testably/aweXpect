using System;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatTypes
{
	[GenerateCollectionExpectation("AreWithinNamespace", "is within namespace {expected}",
		"are within namespace {expected}",
		CollectionOverload = true, NegatedName = "AreNotWithinNamespace",
		Remarks = "Sub-namespaces of the <paramref name=\"expected\" /> namespace are included.",
		NullItemFailsBothWays = true)]
	private static PredicateResult IsWithinNamespaceType(Type type,
		[NegatedParameterName("unexpected")] string expected)
		=> type.IsWithinNamespace(expected);
}
