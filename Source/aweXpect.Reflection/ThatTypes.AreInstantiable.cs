using System;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatTypes
{
	[GenerateCollectionExpectation("AreInstantiable", "is instantiable", "are instantiable",
		CollectionOverload = true, NegatedName = "AreNotInstantiable",
		Remarks = """
		          Abstract types, static types, interfaces and open generic type definitions are not considered instantiable.
		          """,
		NullItemFailsBothWays = true)]
	private static PredicateResult IsInstantiableType(Type type)
		=> type.IsReallyInstantiable();
}
