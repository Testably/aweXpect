using aweXpect.SourceGenerators;

namespace aweXpect;

#if NET8_0_OR_GREATER
[CreateExpectationOn<char>("Is{Not}AnAsciiDigit", "char.IsAsciiDigit({value})",
	ExpectationText = "is {not} an ASCII digit",
	Remarks = """
	          This means, that the specified Unicode character is categorized as an ASCII digit.<br />
	          <seealso cref="char.IsAsciiDigit(char)" />
	          """
)]
#else
[CreateExpectationOn<char>("Is{Not}AnAsciiDigit", "{value} is >= '0' and <= '9'",
	ExpectationText = "is {not} an ASCII digit"
)]
#endif
public static partial class ThatChar;
