using aweXpect.SourceGenerators;

namespace aweXpect;

[CreateExpectationOnNullable<char>("Is{Not}ALetter", "char.IsLetter({value}.Value)",
	ExpectationText = "is {not} a letter",
	Remarks = """
	          This means that the specified Unicode character is categorized as a Unicode letter.<br />
	          <seealso cref="char.IsLetter(char)" />
	          """,
	NegatedRemarks = """
	                 This means that the specified Unicode character is not categorized as a Unicode letter.<br />
	                 <see langword="null" /> is neither treated as a letter nor as not a letter, so it fails.<br />
	                 <seealso cref="char.IsLetter(char)" />
	                 """
)]
public static partial class ThatNullableChar;
