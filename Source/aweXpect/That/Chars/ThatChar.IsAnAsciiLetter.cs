using aweXpect.SourceGenerators;

namespace aweXpect;

#if NET8_0_OR_GREATER
[CreateExpectationOn<char>("Is{Not}AnAsciiLetter", "char.IsAsciiLetter({value})",
	ExpectationText = "is {not} an ASCII letter",
	Remarks = """
	          This means that the specified Unicode character is categorized as an ASCII letter.<br />
	          <seealso cref="char.IsAsciiLetter(char)" />
	          """
)]
#else
[CreateExpectationOn<char>("Is{Not}AnAsciiLetter", "{value} is >= 'a' and <= 'z' or >= 'A' and <= 'Z'",
	ExpectationText = "is {not} an ASCII letter"
)]
#endif
public static partial class ThatChar;
