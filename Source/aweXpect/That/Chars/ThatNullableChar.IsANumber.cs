using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Results;
using aweXpect.SourceGenerators;

namespace aweXpect;

[CreateExpectationOnNullable<char>("Is{Not}ANumber", "char.IsNumber({value}.Value)",
	ExpectationText = "is {not} a number",
	Remarks = """
	          This means that the specified Unicode character is categorized as a number.<br />
	          <seealso cref="char.IsNumber(char)" />
	          """
)]
public static partial class ThatNullableChar;
