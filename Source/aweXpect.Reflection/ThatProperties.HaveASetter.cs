using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatProperties
{
	[GenerateCollectionExpectation("HaveASetter", "has a setter", "have a setter",
		CollectionOverload = true, NegatedName = "DoNotHaveASetter", FormattedItem = nameof(Formattable),
		NullItemFailsBothWays = true)]
	private static PredicateResult HasASetterProperty(PropertyInfo property)
		=> property.HasSetter();
}
