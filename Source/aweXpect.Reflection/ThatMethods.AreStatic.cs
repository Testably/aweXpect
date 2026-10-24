using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatMethods
{
	[GenerateCollectionExpectation("AreStatic", "is static", "are static",
		OnCollection = true, NegatedName = "AreNotStatic", FormattedItem = nameof(Formattable))]
	private static PredicateResult IsStaticMethod(MethodInfo? method)
		=> method is null ? ItemResults.Null : method.IsStatic;
}
