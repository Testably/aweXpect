using System.Reflection;
using aweXpect.Core.Extending;

namespace aweXpect;

public static partial class ThatFields
{
	[GenerateCollectionExpectation("AreReadOnly", "is read-only", "are read-only",
		CollectionOverload = true, NegatedName = "AreNotReadOnly", FormattedItem = nameof(Formattable),
		NullItemFailsBothWays = true)]
	private static PredicateResult IsReadOnlyField(FieldInfo field)
		=> field.IsInitOnly;
}
