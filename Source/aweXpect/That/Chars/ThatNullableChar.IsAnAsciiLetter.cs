using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Results;
using aweXpect.SourceGenerators;

namespace aweXpect;

#if NET8_0_OR_GREATER
[CreateExpectationOnNullable<char>("Is{Not}AnAsciiLetter", "char.IsAsciiLetter({value}.Value)",
	ExpectationText = "is {not} an ASCII letter",
	Remarks = """
	          This means that the specified Unicode character is categorized as an ASCII letter.<br />
	          <seealso cref="char.IsAsciiLetter(char)" />
	          """,
	NegatedRemarks = """
	                 This means that the specified Unicode character is not categorized as an ASCII letter.<br />
	                 <see langword="null" /> is neither treated as an ASCII letter nor as not an ASCII letter, so it fails.<br />
	                 <seealso cref="char.IsAsciiLetter(char)" />
	                 """
)]
#else
[CreateExpectationOnNullable<char>("Is{Not}AnAsciiLetter", "{value}.Value is >= 'a' and <= 'z' or >= 'A' and <= 'Z'",
	ExpectationText = "is {not} an ASCII letter",
	NegatedRemarks = "<see langword=\"null\" /> is neither treated as an ASCII letter nor as not an ASCII letter, so it fails."
)]
#endif
public static partial class ThatNullableChar;
