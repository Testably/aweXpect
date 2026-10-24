using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatFields
{
	[GenerateCollectionExpectation("AreReadOnly", "is read-only", "are read-only",
		CollectionOverload = true, NegatedName = "AreNotReadOnly", FormattedItem = nameof(Formattable))]
	private static PredicateResult IsReadOnlyField(FieldInfo? field)
		=> field is null ? ItemResults.Null : field.IsInitOnly;
}
