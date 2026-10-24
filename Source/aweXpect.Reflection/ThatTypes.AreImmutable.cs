using System;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatTypes
{
	[GenerateCollectionExpectation("AreImmutable", "is immutable", "are immutable",
		CollectionOverload = true, NegatedName = "AreNotImmutable",
		Remarks = """
		          A type is considered immutable when all instance fields (including inherited ones) are
		          <see langword="readonly" /> and all instance properties (including inherited ones) have no setter
		          or an init-only setter.
		          """)]
	private static PredicateResult IsImmutableType(Type? type)
		=> type is null ? ItemResults.Null : type.IsImmutable();
}
