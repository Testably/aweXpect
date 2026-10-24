using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatProperty
{
	[GenerateExpectation("Is{Not}Nullable", "is {not} nullable",
		Result = nameof(DescribeNullableProperty),
		Summary = "Verifies that the <see cref=\"PropertyInfo\" /> is nullable.",
		NegatedSummary = "Verifies that the <see cref=\"PropertyInfo\" /> is not nullable.",
		Remarks = """
		          A property is considered nullable if its type is a <see cref="System.Nullable{T}" /> value type or a
		          reference type annotated as nullable (according to the nullable reference type metadata).
		          """,
		NegatedRemarks = """
		                 A property is considered nullable if its type is a <see cref="System.Nullable{T}" /> value type or a
		                 reference type annotated as nullable (according to the nullable reference type metadata).
		                 Properties without nullability annotations (oblivious code) count as non-nullable.
		                 """)]
	private static bool IsNullableProperty(PropertyInfo property) => property.IsNullable();

	private static string DescribeNullableProperty(PropertyInfo property)
		=> ResultTexts.Describe(property.IsNullable(), "was nullable", "was non-nullable", property);
}
