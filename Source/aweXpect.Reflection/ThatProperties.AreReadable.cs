using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatProperties
{
	[GenerateCollectionExpectation("AreReadable", "is readable", "are readable",
		CollectionOverload = true, FormattedItem = nameof(Formattable),
		Remarks = """
		          Every property has at least one accessor, so
		          <see cref="AreWriteOnly(global::aweXpect.Core.IThat{global::System.Collections.Generic.IEnumerable{global::System.Reflection.PropertyInfo}})" />
		          is the negation.
		          """,
		NullItemFailsBothWays = true)]
	private static PredicateResult IsReadableProperty(PropertyInfo property)
		=> property.IsReadable();
}
