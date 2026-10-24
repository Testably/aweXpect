using System;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatTypes
{
	[GenerateCollectionExpectation("AreInstantiable", "is instantiable", "are instantiable",
		OnCollection = true, NegatedName = "AreNotInstantiable",
		Remarks = """
		          Abstract types, static types, interfaces and open generic type definitions are not considered instantiable.
		          """)]
	private static PredicateResult IsInstantiableType(Type? type)
		=> type is null ? ItemResults.Null : type.IsReallyInstantiable();
}
