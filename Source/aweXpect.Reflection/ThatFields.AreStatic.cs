using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatFields
{
	[GenerateCollectionExpectation("AreStatic", "is static", "are static",
		CollectionOverload = true, NegatedName = "AreNotStatic", FormattedItem = nameof(Formattable))]
	private static PredicateResult IsStaticField(FieldInfo? field)
		=> field is null ? ItemResults.Null : field.IsStatic;
}
