using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Results;
using aweXpect.SourceGenerators;

namespace aweXpect;

[CreateExpectationOnNullable<char>("Is{Not}ALetter", "char.IsLetter({value}.Value)",
	ExpectationText = "is {not} a letter",
	Remarks = """
	          This means that the specified Unicode character is categorized as a Unicode letter.<br />
	          <seealso cref="char.IsLetter(char)" />
	          """
)]
public static partial class ThatNullableChar;
