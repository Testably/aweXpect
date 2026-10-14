using System.Diagnostics.CodeAnalysis;
using aweXpect.Core.Extending;

namespace aweXpect;

public static partial class ThatNullableChar
{
#if NET8_0_OR_GREATER
	[GenerateExpectation("Is{Not}AnAsciiHexDigit", "is {not} an ASCII hex digit",
		Remarks = """
		          This means that the specified Unicode character is categorized as an ASCII hexadecimal digit.<br />
		          <seealso cref="char.IsAsciiHexDigit(char)" />
		          """,
		NegatedRemarks = """
		                 This means that the specified Unicode character is not categorized as an ASCII hexadecimal digit.<br />
		                 <see langword="null" /> is neither treated as an ASCII hex digit nor as not an ASCII hex digit, so it fails.<br />
		                 <seealso cref="char.IsAsciiHexDigit(char)" />
		                 """)]
	private static bool IsAsciiHexDigit([DisallowNull] char? value) => char.IsAsciiHexDigit(value.Value);
#else
	[GenerateExpectation("Is{Not}AnAsciiHexDigit", "is {not} an ASCII hex digit",
		NegatedRemarks = "<see langword=\"null\" /> is neither treated as an ASCII hex digit nor as not an ASCII hex digit, so it fails.")]
	private static bool IsAsciiHexDigit([DisallowNull] char? value)
		=> value.Value is >= '0' and <= '9' or >= 'a' and <= 'f' or >= 'A' and <= 'F';
#endif
}
