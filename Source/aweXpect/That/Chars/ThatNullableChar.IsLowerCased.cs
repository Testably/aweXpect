using System.Diagnostics.CodeAnalysis;
using aweXpect.Core.Extending;

namespace aweXpect;

public static partial class ThatNullableChar
{
	[GenerateExpectation("Is{Not}LowerCased", "is {not} lower-cased",
		Remarks = """
		          This means that the specified Unicode character is categorized as a lowercase letter.<br />
		          Unlike for a <see langword="string" />, a character that is not a letter (e.g. <c>'1'</c>) is not lower-cased.<br />
		          <seealso cref="char.IsLower(char)" />
		          """,
		NegatedRemarks = """
		                 This means that the specified Unicode character is not categorized as a lowercase letter.<br />
		                 Unlike for a <see langword="string" />, a character that is not a letter (e.g. <c>'1'</c>) is not lower-cased.<br />
		                 <see langword="null" /> is neither treated as lower-cased nor as not lower-cased, so it fails.<br />
		                 <seealso cref="char.IsLower(char)" />
		                 """)]
	private static bool IsLower([DisallowNull] char? value) => char.IsLower(value.Value);
}
