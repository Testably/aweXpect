using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Results;
using aweXpect.SourceGenerators;

namespace aweXpect;

[CreateExpectationOnNullable<char>("Is{Not}Control", "char.IsControl({value}.Value)",
	ExpectationText = "is {not} a control character",
	Remarks = """
	          This means, that the specified Unicode character is categorized as a control character.<br />
	          <seealso cref="char.IsControl(char)" />
	          """
)]
public static partial class ThatNullableChar;
