using aweXpect.SourceGenerators;

namespace aweXpect;

[CreateExpectationOn<char>("Is{Not}ALetter", "char.IsLetter({value})",
	ExpectationText = "is {not} a letter",
	Remarks = """
	          This means that the specified Unicode character is categorized as a Unicode letter.<br />
	          <seealso cref="char.IsLetter(char)" />
	          """
)]
public static partial class ThatChar;
