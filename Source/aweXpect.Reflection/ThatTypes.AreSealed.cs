using System;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatTypes
{
	[GenerateCollectionExpectation("AreSealed", "is sealed", "are sealed",
		CollectionOverload = true, NegatedName = "AreNotSealed",
		Remarks = """
		          Static types are not considered sealed, even though they
		          have <see cref="Type.IsSealed" /> set to <see langword="true" />.
		          """,
		NullItemFailsBothWays = true)]
	private static PredicateResult IsSealedType(Type type)
		=> type.IsReallySealed();
}
