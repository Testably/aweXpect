using System.Diagnostics.CodeAnalysis;
using aweXpect.Core.Extending;

namespace aweXpect;

public static partial class ThatNullableChar
{
	[GenerateExpectation("Is{Not}ADigit", "is {not} a digit",
		Remarks = """
		          This means that the specified Unicode character is categorized as a decimal digit.<br />
		          <seealso cref="char.IsDigit(char)" />
		          """,
		NegatedRemarks = """
		                 This means that the specified Unicode character is not categorized as a decimal digit.<br />
		                 <see langword="null" /> is neither treated as a digit nor as not a digit, so it fails.<br />
		                 <seealso cref="char.IsDigit(char)" />
		                 """)]
	private static bool IsDigit([DisallowNull] char? value) => char.IsDigit(value.Value);
}
