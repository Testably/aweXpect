using aweXpect.Core.Extending;

namespace aweXpect;

public static partial class ThatChar
{
	[GenerateExpectation("Is{Not}UpperCased", "is {not} upper-cased",
		NullableOverload = false,
		Remarks = """
		          This means that the specified Unicode character is categorized as an uppercase letter.<br />
		          Unlike for a <see langword="string" />, a character that is not a letter (e.g. <c>'1'</c>) is not upper-cased.<br />
		          <seealso cref="char.IsUpper(char)" />
		          """,
		NegatedRemarks = """
		                 This means that the specified Unicode character is not categorized as an uppercase letter.<br />
		                 Unlike for a <see langword="string" />, a character that is not a letter (e.g. <c>'1'</c>) is not upper-cased.<br />
		                 <seealso cref="char.IsUpper(char)" />
		                 """)]
	private static bool IsUpper(char value) => char.IsUpper(value);
}
