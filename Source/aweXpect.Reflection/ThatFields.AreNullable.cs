using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatFields
{
	[GenerateCollectionExpectation("AreNullable", "is nullable", "are nullable",
		CollectionOverload = true, NegatedName = "AreNotNullable", FormattedItem = nameof(Formattable))]
	private static PredicateResult IsNullableField(FieldInfo? field)
		=> field is null ? ItemResults.Null : field.IsNullable();
}
