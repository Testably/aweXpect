using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Results;
using aweXpect.SourceGenerators;

namespace aweXpect;

#if NET8_0_OR_GREATER
[CreateExpectationOnNullable<char>("Is{Not}AnAsciiDigit", "char.IsAsciiDigit({value}.Value)",
	ExpectationText = "is {not} an ASCII digit",
	Remarks = """
	          This means, that the specified Unicode character is categorized as an ASCII digit.<br />
	          <seealso cref="char.IsAsciiDigit(char)" />
	          """
)]
#else
[CreateExpectationOnNullable<char>("Is{Not}AnAsciiDigit", "{value}.Value is >= '0' and <= '9'",
	ExpectationText = "is {not} an ASCII digit"
)]
#endif
public static partial class ThatNullableChar;
