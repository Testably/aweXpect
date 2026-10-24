using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatField
{
	[GenerateExpectation("Is{Not}Nullable", "is {not} nullable",
		Result = nameof(DescribeNullableField),
		Summary = "Verifies that the <see cref=\"FieldInfo\" /> is nullable.",
		NegatedSummary = "Verifies that the <see cref=\"FieldInfo\" /> is not nullable.",
		Remarks = """
		          A field is considered nullable if its type is a <see cref="System.Nullable{T}" /> value type or a
		          reference type annotated as nullable (according to the nullable reference type metadata).
		          """,
		NegatedRemarks = """
		                 A field is considered nullable if its type is a <see cref="System.Nullable{T}" /> value type or a
		                 reference type annotated as nullable (according to the nullable reference type metadata).
		                 Fields without nullability annotations (oblivious code) count as non-nullable.
		                 """)]
	private static bool IsNullableField(FieldInfo field) => field.IsNullable();

	private static string DescribeNullableField(FieldInfo field)
		=> ResultTexts.Describe(field.IsNullable(), "was nullable", "was non-nullable", field);
}
