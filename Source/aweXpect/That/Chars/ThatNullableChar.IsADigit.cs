using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Results;
using aweXpect.SourceGenerators;

namespace aweXpect;

[CreateExpectationOnNullable<char>("Is{Not}ADigit", "char.IsDigit({value}.Value)",
	ExpectationText = "is {not} a digit",
	Remarks = """
	          This means that the specified Unicode character is categorized as a decimal digit.<br />
	          <seealso cref="char.IsDigit(char)" />
	          """
)]
public static partial class ThatNullableChar;
