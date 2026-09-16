using aweXpect.SourceGenerators;

namespace aweXpect;

#if NET8_0_OR_GREATER
[CreateExpectationOn<char>("Is{Not}AnAsciiHexDigit", "char.IsAsciiHexDigit({value})",
	ExpectationText = "is {not} an ASCII hex digit",
	Remarks = """
	          This means, that the specified Unicode character is categorized as an ASCII hexadecimal digit.<br />
	          <seealso cref="char.IsAsciiHexDigit(char)" />
	          """
)]
#else
[CreateExpectationOn<char>("Is{Not}AnAsciiHexDigit", "{value} is >= '0' and <= '9' or >= 'a' and <= 'f' or >= 'A' and <= 'F'",
	ExpectationText = "is {not} an ASCII hex digit"
)]
#endif
public static partial class ThatChar;
