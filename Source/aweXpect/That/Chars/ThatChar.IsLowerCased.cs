using aweXpect.SourceGenerators;

namespace aweXpect;

[CreateExpectationOn<char>("Is{Not}LowerCased", "char.IsLower({value})",
	ExpectationText = "is {not} lower-cased",
	Remarks = """
	          This means, that the specified Unicode character is categorized as a lowercase letter.<br />
	          <seealso cref="char.IsLower(char)" />
	          """
)]
public static partial class ThatChar;
