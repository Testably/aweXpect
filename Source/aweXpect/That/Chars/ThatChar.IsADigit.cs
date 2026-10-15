using aweXpect.Core.Extending;

namespace aweXpect;

public static partial class ThatChar
{
	[GenerateExpectation("Is{Not}ADigit", "is {not} a digit",
		NullableOverload = false,
		Remarks = """
		          This means that the specified Unicode character is categorized as a decimal digit.<br />
		          <seealso cref="char.IsDigit(char)" />
		          """,
		NegatedRemarks = """
		                 This means that the specified Unicode character is not categorized as a decimal digit.<br />
		                 <seealso cref="char.IsDigit(char)" />
		                 """)]
	private static bool IsDigit(char value) => char.IsDigit(value);
}
