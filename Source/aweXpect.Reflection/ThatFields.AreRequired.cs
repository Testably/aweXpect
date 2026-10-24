using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatFields
{
	[GenerateCollectionExpectation("AreRequired", "is required", "are required",
		CollectionOverload = true, NegatedName = "AreNotRequired", FormattedItem = nameof(Formattable))]
	private static PredicateResult IsRequiredField(FieldInfo? field)
		=> field is null ? ItemResults.Null : field.IsRequired();
}
