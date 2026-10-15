using System.Diagnostics.CodeAnalysis;
using aweXpect.Core.Extending;

namespace aweXpect;

public static partial class ThatNullableChar
{
	[GenerateExpectation("Is{Not}ANumber", "is {not} a number",
		Remarks = """
		          This means that the specified Unicode character is categorized as a number.<br />
		          <seealso cref="char.IsNumber(char)" />
		          """,
		NegatedRemarks = """
		                 This means that the specified Unicode character is not categorized as a number.<br />
		                 <see langword="null" /> is neither treated as a number nor as not a number, so it fails.<br />
		                 <seealso cref="char.IsNumber(char)" />
		                 """)]
	private static bool IsNumber([DisallowNull] char? value) => char.IsNumber(value.Value);
}
