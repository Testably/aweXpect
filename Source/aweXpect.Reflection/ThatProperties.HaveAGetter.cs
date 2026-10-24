using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatProperties
{
	[GenerateCollectionExpectation("HaveAGetter", "has a getter", "have a getter",
		OnCollection = true, FormattedItem = nameof(Formattable))]
	private static PredicateResult HasAGetterProperty(PropertyInfo? property)
		=> property is null ? ItemResults.Null : property.HasGetter();
}
