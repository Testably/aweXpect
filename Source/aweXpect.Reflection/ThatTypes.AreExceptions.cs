using System;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatTypes
{
	[GenerateCollectionExpectation("AreExceptions", "is an exception", "are exceptions",
		CollectionOverload = true, NegatedName = "AreNotExceptions",
		NullItemFailsBothWays = true)]
	private static PredicateResult IsAnExceptionType(Type type)
		=> type.IsException();
}
