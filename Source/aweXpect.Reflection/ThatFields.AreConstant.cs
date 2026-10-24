using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatFields
{
	[GenerateCollectionExpectation("AreConstant", "is constant", "are constant",
		OnCollection = true, NegatedName = "AreNotConstant", FormattedItem = nameof(Formattable))]
	private static PredicateResult IsConstantField(FieldInfo? field)
		=> field is null ? ItemResults.Null : field.IsLiteral;
}
