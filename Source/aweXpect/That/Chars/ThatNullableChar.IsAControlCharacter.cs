using System.Diagnostics.CodeAnalysis;
using aweXpect.Core.Extending;

namespace aweXpect;

public static partial class ThatNullableChar
{
	[GenerateExpectation("Is{Not}AControlCharacter", "is {not} a control character",
		Remarks = """
		          This means that the specified Unicode character is categorized as a control character.<br />
		          <seealso cref="char.IsControl(char)" />
		          """,
		NegatedRemarks = """
		                 This means that the specified Unicode character is not categorized as a control character.<br />
		                 <see langword="null" /> is neither treated as a control character nor as not a control character, so it fails.<br />
		                 <seealso cref="char.IsControl(char)" />
		                 """)]
	private static bool IsControl([DisallowNull] char? value) => char.IsControl(value.Value);
}
