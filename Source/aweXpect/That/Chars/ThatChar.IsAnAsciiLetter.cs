using aweXpect.Core.Extending;

namespace aweXpect;

public static partial class ThatChar
{
#if NET8_0_OR_GREATER
	[GenerateExpectation("Is{Not}AnAsciiLetter", "is {not} an ASCII letter",
		NullableOverload = false,
		Remarks = """
		          This means that the specified Unicode character is categorized as an ASCII letter.<br />
		          <seealso cref="char.IsAsciiLetter(char)" />
		          """,
		NegatedRemarks = """
		                 This means that the specified Unicode character is not categorized as an ASCII letter.<br />
		                 <seealso cref="char.IsAsciiLetter(char)" />
		                 """)]
	private static bool IsAsciiLetter(char value) => char.IsAsciiLetter(value);
#else
	[GenerateExpectation("Is{Not}AnAsciiLetter", "is {not} an ASCII letter",
		NullableOverload = false)]
	private static bool IsAsciiLetter(char value) => value is >= 'a' and <= 'z' or >= 'A' and <= 'Z';
#endif
}
