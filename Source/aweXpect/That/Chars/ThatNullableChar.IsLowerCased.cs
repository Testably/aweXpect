using aweXpect.SourceGenerators;

namespace aweXpect;

[CreateExpectationOnNullable<char>("Is{Not}LowerCased", "char.IsLower({value}.Value)",
	ExpectationText = "is {not} lower-cased",
	Remarks = """
	          This means that the specified Unicode character is categorized as a lowercase letter.<br />
	          Unlike for a <see langword="string" />, a character that is not a letter (e.g. <c>'1'</c>) is not lower-cased.<br />
	          <seealso cref="char.IsLower(char)" />
	          """,
	NegatedRemarks = """
	                 This means that the specified Unicode character is not categorized as a lowercase letter.<br />
	                 Unlike for a <see langword="string" />, a character that is not a letter (e.g. <c>'1'</c>) is not lower-cased.<br />
	                 <see langword="null" /> is neither treated as lower-cased nor as not lower-cased, so it fails.<br />
	                 <seealso cref="char.IsLower(char)" />
	                 """
)]
public static partial class ThatNullableChar;
