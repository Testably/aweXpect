using System.Diagnostics.CodeAnalysis;
using aweXpect.Core.Extending;

namespace aweXpect;

public static partial class ThatNullableChar
{
	[GenerateExpectation("Is{Not}UpperCased", "is {not} upper-cased",
		Remarks = """
		          This means that the specified Unicode character is categorized as an uppercase letter.<br />
		          Unlike for a <see langword="string" />, a character that is not a letter (e.g. <c>'1'</c>) is not upper-cased.<br />
		          <seealso cref="char.IsUpper(char)" />
		          """,
		NegatedRemarks = """
		                 This means that the specified Unicode character is not categorized as an uppercase letter.<br />
		                 Unlike for a <see langword="string" />, a character that is not a letter (e.g. <c>'1'</c>) is not upper-cased.<br />
		                 <see langword="null" /> is neither treated as upper-cased nor as not upper-cased, so it fails.<br />
		                 <seealso cref="char.IsUpper(char)" />
		                 """)]
	private static bool IsUpper([DisallowNull] char? value) => char.IsUpper(value.Value);
}
