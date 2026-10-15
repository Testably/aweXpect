using aweXpect.Core.Extending;

namespace aweXpect;

public static partial class ThatChar
{
	[GenerateExpectation("Is{Not}ANumber", "is {not} a number",
		NullableOverload = false,
		Remarks = """
		          This means that the specified Unicode character is categorized as a number.<br />
		          <seealso cref="char.IsNumber(char)" />
		          """,
		NegatedRemarks = """
		                 This means that the specified Unicode character is not categorized as a number.<br />
		                 <seealso cref="char.IsNumber(char)" />
		                 """)]
	private static bool IsNumber(char value) => char.IsNumber(value);
}
