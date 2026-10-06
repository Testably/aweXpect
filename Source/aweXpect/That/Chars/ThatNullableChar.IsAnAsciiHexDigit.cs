using aweXpect.SourceGenerators;

namespace aweXpect;

#if NET8_0_OR_GREATER
[CreateExpectationOnNullable<char>("Is{Not}AnAsciiHexDigit", "char.IsAsciiHexDigit({value}.Value)",
	ExpectationText = "is {not} an ASCII hex digit",
	Remarks = """
	          This means that the specified Unicode character is categorized as an ASCII hexadecimal digit.<br />
	          <seealso cref="char.IsAsciiHexDigit(char)" />
	          """,
	NegatedRemarks = """
	                 This means that the specified Unicode character is not categorized as an ASCII hexadecimal digit.<br />
	                 <see langword="null" /> is neither treated as an ASCII hex digit nor as not an ASCII hex digit, so it fails.<br />
	                 <seealso cref="char.IsAsciiHexDigit(char)" />
	                 """
)]
#else
[CreateExpectationOnNullable<char>("Is{Not}AnAsciiHexDigit", "{value}.Value is >= '0' and <= '9' or >= 'a' and <= 'f' or >= 'A' and <= 'F'",
	ExpectationText = "is {not} an ASCII hex digit",
	NegatedRemarks = "<see langword=\"null\" /> is neither treated as an ASCII hex digit nor as not an ASCII hex digit, so it fails."
)]
#endif
public static partial class ThatNullableChar;
