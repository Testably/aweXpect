using aweXpect.SourceGenerators;

namespace aweXpect;

[CreateExpectationOnNullable<char>("Is{Not}AControlCharacter", "char.IsControl({value}.Value)",
	ExpectationText = "is {not} a control character",
	Remarks = """
	          This means that the specified Unicode character is categorized as a control character.<br />
	          <seealso cref="char.IsControl(char)" />
	          """,
	NegatedRemarks = """
	                 This means that the specified Unicode character is not categorized as a control character.<br />
	                 <see langword="null" /> is neither treated as a control character nor as not a control character, so it fails.<br />
	                 <seealso cref="char.IsControl(char)" />
	                 """
)]
public static partial class ThatNullableChar;
