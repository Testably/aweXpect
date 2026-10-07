using aweXpect.SourceGenerators;

namespace aweXpect;

[CreateExpectationOnNullable<char>("Is{Not}ADigit", "char.IsDigit({value}.Value)",
	ExpectationText = "is {not} a digit",
	Remarks = """
	          This means that the specified Unicode character is categorized as a decimal digit.<br />
	          <seealso cref="char.IsDigit(char)" />
	          """,
	NegatedRemarks = """
	                 This means that the specified Unicode character is not categorized as a decimal digit.<br />
	                 <see langword="null" /> is neither treated as a digit nor as not a digit, so it fails.<br />
	                 <seealso cref="char.IsDigit(char)" />
	                 """
)]
public static partial class ThatNullableChar;
