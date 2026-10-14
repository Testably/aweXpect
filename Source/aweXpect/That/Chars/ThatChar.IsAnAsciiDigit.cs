using aweXpect.Core.Extending;

namespace aweXpect;

public static partial class ThatChar
{
#if NET8_0_OR_GREATER
	[GenerateExpectation("Is{Not}AnAsciiDigit", "is {not} an ASCII digit",
		NullableOverload = false,
		Remarks = """
		          This means that the specified Unicode character is categorized as an ASCII digit.<br />
		          <seealso cref="char.IsAsciiDigit(char)" />
		          """,
		NegatedRemarks = """
		                 This means that the specified Unicode character is not categorized as an ASCII digit.<br />
		                 <seealso cref="char.IsAsciiDigit(char)" />
		                 """)]
	private static bool IsAsciiDigit(char value) => char.IsAsciiDigit(value);
#else
	[GenerateExpectation("Is{Not}AnAsciiDigit", "is {not} an ASCII digit",
		NullableOverload = false)]
	private static bool IsAsciiDigit(char value) => value is >= '0' and <= '9';
#endif
}
