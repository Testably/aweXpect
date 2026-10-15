using aweXpect.Core.Extending;

namespace aweXpect;

public static partial class ThatChar
{
	[GenerateExpectation("Is{Not}ALetter", "is {not} a letter",
		NullableOverload = false,
		Remarks = """
		          This means that the specified Unicode character is categorized as a Unicode letter.<br />
		          <seealso cref="char.IsLetter(char)" />
		          """,
		NegatedRemarks = """
		                 This means that the specified Unicode character is not categorized as a Unicode letter.<br />
		                 <seealso cref="char.IsLetter(char)" />
		                 """)]
	private static bool IsLetter(char value) => char.IsLetter(value);
}
