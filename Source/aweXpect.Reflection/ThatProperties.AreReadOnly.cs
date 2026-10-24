using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatProperties
{
	[GenerateCollectionExpectation("AreReadOnly", "is read-only", "are read-only",
		OnCollection = true, FormattedItem = nameof(Formattable))]
	private static PredicateResult IsReadOnlyProperty(PropertyInfo? property)
		=> property is null ? ItemResults.Null : property.IsReadOnly();
}
