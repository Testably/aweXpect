using System.Reflection;
using aweXpect.Core.Extending;

namespace aweXpect;

public static partial class ThatFields
{
	[GenerateCollectionExpectation("AreStatic", "is static", "are static",
		CollectionOverload = true, NegatedName = "AreNotStatic", FormattedItem = nameof(Formattable),
		NullItemFailsBothWays = true)]
	private static PredicateResult IsStaticField(FieldInfo field)
		=> field.IsStatic;
}
