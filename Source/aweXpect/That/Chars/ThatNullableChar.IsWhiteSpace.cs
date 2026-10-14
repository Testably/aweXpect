using System.Diagnostics.CodeAnalysis;
using aweXpect.Core.Extending;

namespace aweXpect;

public static partial class ThatNullableChar
{
	[GenerateExpectation("Is{Not}WhiteSpace", "is {not} whitespace",
		Remarks = """
		          This means that the specified Unicode character is categorized as whitespace.<br />
		          <seealso cref="char.IsWhiteSpace(char)" />
		          """,
		NegatedRemarks = """
		                 This means that the specified Unicode character is not categorized as whitespace.<br />
		                 <see langword="null" /> is neither treated as whitespace nor as not whitespace, so it fails.<br />
		                 <seealso cref="char.IsWhiteSpace(char)" />
		                 """)]
	private static bool IsWhiteSpaceCharacter([DisallowNull] char? value) => char.IsWhiteSpace(value.Value);
}
