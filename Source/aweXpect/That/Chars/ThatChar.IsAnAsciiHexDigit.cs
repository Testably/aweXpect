using aweXpect.Core.Extending;

namespace aweXpect;

public static partial class ThatChar
{
#if NET8_0_OR_GREATER
	[GenerateExpectation("Is{Not}AnAsciiHexDigit", "is {not} an ASCII hex digit",
		NullableOverload = false,
		Remarks = """
		          This means that the specified Unicode character is categorized as an ASCII hexadecimal digit.<br />
		          <seealso cref="char.IsAsciiHexDigit(char)" />
		          """,
		NegatedRemarks = """
		                 This means that the specified Unicode character is not categorized as an ASCII hexadecimal digit.<br />
		                 <seealso cref="char.IsAsciiHexDigit(char)" />
		                 """)]
	private static bool IsAsciiHexDigit(char value) => char.IsAsciiHexDigit(value);
#else
	[GenerateExpectation("Is{Not}AnAsciiHexDigit", "is {not} an ASCII hex digit",
		NullableOverload = false)]
	private static bool IsAsciiHexDigit(char value)
		=> value is >= '0' and <= '9' or >= 'a' and <= 'f' or >= 'A' and <= 'F';
#endif
}
