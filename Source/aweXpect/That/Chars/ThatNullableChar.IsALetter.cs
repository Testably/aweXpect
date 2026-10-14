using System.Diagnostics.CodeAnalysis;
using aweXpect.Core.Extending;

namespace aweXpect;

public static partial class ThatNullableChar
{
	[GenerateExpectation("Is{Not}ALetter", "is {not} a letter",
		Remarks = """
		          This means that the specified Unicode character is categorized as a Unicode letter.<br />
		          <seealso cref="char.IsLetter(char)" />
		          """,
		NegatedRemarks = """
		                 This means that the specified Unicode character is not categorized as a Unicode letter.<br />
		                 <see langword="null" /> is neither treated as a letter nor as not a letter, so it fails.<br />
		                 <seealso cref="char.IsLetter(char)" />
		                 """)]
	private static bool IsLetter([DisallowNull] char? value) => char.IsLetter(value.Value);
}
