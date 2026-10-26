using System.Reflection;
using aweXpect.Core.Extending;

namespace aweXpect;

public static partial class ThatFields
{
	[GenerateCollectionExpectation("AreConstant", "is constant", "are constant",
		CollectionOverload = true, NegatedName = "AreNotConstant", FormattedItem = nameof(Formattable),
		NullItemFailsBothWays = true)]
	private static PredicateResult IsConstantField(FieldInfo field)
		=> field.IsLiteral;
}
