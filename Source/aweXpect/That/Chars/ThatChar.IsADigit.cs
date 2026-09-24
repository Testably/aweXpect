using aweXpect.SourceGenerators;

namespace aweXpect;

[CreateExpectationOn<char>("Is{Not}ADigit", "char.IsDigit({value})",
	ExpectationText = "is {not} a digit",
	Remarks = """
	          This means that the specified Unicode character is categorized as a decimal digit.<br />
	          <seealso cref="char.IsDigit(char)" />
	          """
)]
public static partial class ThatChar;
