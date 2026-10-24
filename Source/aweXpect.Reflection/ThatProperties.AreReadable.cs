using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatProperties
{
	[GenerateCollectionExpectation("AreReadable", "is readable", "are readable",
		OnCollection = true, FormattedItem = nameof(Formattable))]
	private static PredicateResult IsReadableProperty(PropertyInfo? property)
		=> property is null ? ItemResults.Null : property.IsReadable();
}
