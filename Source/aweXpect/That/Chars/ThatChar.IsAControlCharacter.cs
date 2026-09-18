using aweXpect.SourceGenerators;

namespace aweXpect;

[CreateExpectationOn<char>("Is{Not}AControlCharacter", "char.IsControl({value})",
	ExpectationText = "is {not} a control character",
	Remarks = """
	          This means, that the specified Unicode character is categorized as a control character.<br />
	          <seealso cref="char.IsControl(char)" />
	          """
)]
public static partial class ThatChar;
