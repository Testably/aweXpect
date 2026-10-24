using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatProperties
{
	[GenerateCollectionExpectation("AreWritable", "is writable", "are writable",
		OnCollection = true, FormattedItem = nameof(Formattable))]
	private static PredicateResult IsWritableProperty(PropertyInfo? property)
		=> property is null ? ItemResults.Null : property.IsWritable();
}
