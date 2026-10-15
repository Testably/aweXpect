using aweXpect.Core.Extending;

namespace aweXpect;

public static partial class ThatChar
{
	[GenerateExpectation("Is{Not}AControlCharacter", "is {not} a control character",
		NullableOverload = false,
		Remarks = """
		          This means that the specified Unicode character is categorized as a control character.<br />
		          <seealso cref="char.IsControl(char)" />
		          """,
		NegatedRemarks = """
		                 This means that the specified Unicode character is not categorized as a control character.<br />
		                 <seealso cref="char.IsControl(char)" />
		                 """)]
	private static bool IsControl(char value) => char.IsControl(value);
}
