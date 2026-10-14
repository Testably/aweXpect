using System.Diagnostics.CodeAnalysis;
using aweXpect.Core.Extending;

namespace aweXpect;

public static partial class ThatNullableChar
{
#if NET8_0_OR_GREATER
	[GenerateExpectation("Is{Not}AnAsciiLetter", "is {not} an ASCII letter",
		Remarks = """
		          This means that the specified Unicode character is categorized as an ASCII letter.<br />
		          <seealso cref="char.IsAsciiLetter(char)" />
		          """,
		NegatedRemarks = """
		                 This means that the specified Unicode character is not categorized as an ASCII letter.<br />
		                 <see langword="null" /> is neither treated as an ASCII letter nor as not an ASCII letter, so it fails.<br />
		                 <seealso cref="char.IsAsciiLetter(char)" />
		                 """)]
	private static bool IsAsciiLetter([DisallowNull] char? value) => char.IsAsciiLetter(value.Value);
#else
	[GenerateExpectation("Is{Not}AnAsciiLetter", "is {not} an ASCII letter",
		NegatedRemarks = "<see langword=\"null\" /> is neither treated as an ASCII letter nor as not an ASCII letter, so it fails.")]
	private static bool IsAsciiLetter([DisallowNull] char? value) => value.Value is >= 'a' and <= 'z' or >= 'A' and <= 'Z';
#endif
}
