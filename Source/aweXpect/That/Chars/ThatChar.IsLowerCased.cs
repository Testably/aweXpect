using aweXpect.Core.Extending;

namespace aweXpect;

public static partial class ThatChar
{
	[GenerateExpectation("Is{Not}LowerCased", "is {not} lower-cased",
		NullableOverload = false,
		Remarks = """
		          This means that the specified Unicode character is categorized as a lowercase letter.<br />
		          Unlike for a <see langword="string" />, a character that is not a letter (e.g. <c>'1'</c>) is not lower-cased.<br />
		          <seealso cref="char.IsLower(char)" />
		          """,
		NegatedRemarks = """
		                 This means that the specified Unicode character is not categorized as a lowercase letter.<br />
		                 Unlike for a <see langword="string" />, a character that is not a letter (e.g. <c>'1'</c>) is not lower-cased.<br />
		                 <seealso cref="char.IsLower(char)" />
		                 """)]
	private static bool IsLower(char value) => char.IsLower(value);
}
