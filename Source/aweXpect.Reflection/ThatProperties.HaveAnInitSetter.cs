using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatProperties
{
	[GenerateCollectionExpectation("HaveAnInitSetter", "has an init setter", "have an init setter",
		CollectionOverload = true, NegatedName = "DoNotHaveAnInitSetter", FormattedItem = nameof(Formattable),
		NullItemFailsBothWays = true)]
	private static PredicateResult HasAnInitSetterProperty(PropertyInfo property)
		=> property.HasInitSetter();
}
