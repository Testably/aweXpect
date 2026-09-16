using aweXpect.SourceGenerators;

namespace aweXpect;

[CreateExpectationOn<char>("Is{Not}UpperCased", "char.IsUpper({value})",
	ExpectationText = "is {not} upper-cased",
	Remarks = """
	          This means, that the specified Unicode character is categorized as an uppercase letter.<br />
	          <seealso cref="char.IsUpper(char)" />
	          """
)]
public static partial class ThatChar;
