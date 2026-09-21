using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Results;
using aweXpect.SourceGenerators;

namespace aweXpect;

[CreateExpectationOnNullable<char>("Is{Not}WhiteSpace", "char.IsWhiteSpace({value}.Value)",
	ExpectationText = "is {not} whitespace",
	Remarks = """
	          This means, that the specified Unicode character is categorized as white-space.<br />
	          <seealso cref="char.IsWhiteSpace(char)" />
	          """
)]
public static partial class ThatNullableChar;
