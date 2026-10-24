using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatProperties
{
	[GenerateCollectionExpectation("HaveASetter", "has a setter", "have a setter",
		OnCollection = true, FormattedItem = nameof(Formattable))]
	private static PredicateResult HasASetterProperty(PropertyInfo? property)
		=> property is null ? ItemResults.Null : property.HasSetter();
}
