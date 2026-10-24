using System;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatTypes
{
	[GenerateCollectionExpectation("AreAbstract", "is abstract", "are abstract",
		OnCollection = true, NegatedName = "AreNotAbstract",
		Remarks = """
		          Static types or interfaces are not considered abstract, even though they
		          have <see cref="Type.IsAbstract" /> set to <see langword="true" />.
		          """)]
	private static PredicateResult IsAbstractType(Type? type)
		=> type is null ? ItemResults.Null : type.IsReallyAbstract();
}
