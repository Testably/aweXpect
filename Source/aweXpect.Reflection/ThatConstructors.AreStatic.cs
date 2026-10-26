using System.Reflection;
using aweXpect.Core.Extending;

namespace aweXpect;

public static partial class ThatConstructors
{
	[GenerateCollectionExpectation("AreStatic", "is static", "are static",
		CollectionOverload = true, NegatedName = "AreNotStatic", FormattedItem = nameof(Formattable),
		NullItemFailsBothWays = true)]
	private static PredicateResult IsStaticConstructor(ConstructorInfo constructor)
		=> constructor.IsStatic;
}
