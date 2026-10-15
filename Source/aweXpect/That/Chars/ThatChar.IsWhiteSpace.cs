using aweXpect.Core.Extending;

namespace aweXpect;

public static partial class ThatChar
{
	[GenerateExpectation("Is{Not}WhiteSpace", "is {not} whitespace",
		NullableOverload = false,
		Remarks = """
		          This means that the specified Unicode character is categorized as whitespace.<br />
		          <seealso cref="char.IsWhiteSpace(char)" />
		          """,
		NegatedRemarks = """
		                 This means that the specified Unicode character is not categorized as whitespace.<br />
		                 <seealso cref="char.IsWhiteSpace(char)" />
		                 """)]
	private static bool IsWhiteSpaceCharacter(char value) => char.IsWhiteSpace(value);
}
