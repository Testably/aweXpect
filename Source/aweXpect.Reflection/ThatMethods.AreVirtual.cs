using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatMethods
{
	[GenerateCollectionExpectation("AreVirtual", "is virtual", "are virtual",
		OnCollection = true, NegatedName = "AreNotVirtual", FormattedItem = nameof(Formattable))]
	private static PredicateResult IsVirtualMethod(MethodInfo? method)
		=> method is null ? ItemResults.Null : method.IsVirtual;
}
