using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatConstructors
{
	[GenerateCollectionExpectation("AreStatic", "is static", "are static",
		OnCollection = true, NegatedName = "AreNotStatic", FormattedItem = nameof(Formattable))]
	private static PredicateResult IsStaticConstructor(ConstructorInfo? constructor)
		=> constructor is null ? ItemResults.Null : constructor.IsStatic;
}
