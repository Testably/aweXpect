using aweXpect.SourceGenerators;

namespace aweXpect;

[CreateExpectationOn<char>("Is{Not}ANumber", "char.IsNumber({value})",
	ExpectationText = "is {not} a number",
	Remarks = """
	          This means that the specified Unicode character is categorized as a number.<br />
	          <seealso cref="char.IsNumber(char)" />
	          """
)]
public static partial class ThatChar;
