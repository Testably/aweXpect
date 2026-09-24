using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Results;
using aweXpect.SourceGenerators;

namespace aweXpect;

[CreateExpectationOnNullable<char>("Is{Not}UpperCased", "char.IsUpper({value}.Value)",
	ExpectationText = "is {not} upper-cased",
	Remarks = """
	          This means that the specified Unicode character is categorized as an uppercase letter.<br />
	          <seealso cref="char.IsUpper(char)" />
	          """
)]
public static partial class ThatNullableChar;
