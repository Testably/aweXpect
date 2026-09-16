using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Results;
using aweXpect.SourceGenerators;

namespace aweXpect;

[CreateExpectationOnNullable<char>("Is{Not}LowerCased", "char.IsLower({value}.Value)",
	ExpectationText = "is {not} lower-cased",
	Remarks = """
	          This means, that the specified Unicode character is categorized as a lowercase letter.<br />
	          <seealso cref="char.IsLower(char)" />
	          """
)]
public static partial class ThatNullableChar;
