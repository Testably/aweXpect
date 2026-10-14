using System.Diagnostics.CodeAnalysis;
using aweXpect.Core.Extending;

namespace aweXpect;

public static partial class ThatNullableChar
{
#if NET8_0_OR_GREATER
	[GenerateExpectation("Is{Not}AnAsciiDigit", "is {not} an ASCII digit",
		Remarks = """
		          This means that the specified Unicode character is categorized as an ASCII digit.<br />
		          <seealso cref="char.IsAsciiDigit(char)" />
		          """,
		NegatedRemarks = """
		                 This means that the specified Unicode character is not categorized as an ASCII digit.<br />
		                 <see langword="null" /> is neither treated as an ASCII digit nor as not an ASCII digit, so it fails.<br />
		                 <seealso cref="char.IsAsciiDigit(char)" />
		                 """)]
	private static bool IsAsciiDigit([DisallowNull] char? value) => char.IsAsciiDigit(value.Value);
#else
	[GenerateExpectation("Is{Not}AnAsciiDigit", "is {not} an ASCII digit",
		NegatedRemarks = "<see langword=\"null\" /> is neither treated as an ASCII digit nor as not an ASCII digit, so it fails.")]
	private static bool IsAsciiDigit([DisallowNull] char? value) => value.Value is >= '0' and <= '9';
#endif
}
