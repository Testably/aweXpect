using System;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatType
{
	[GenerateExpectation("Is{Not}Sealed", "is {not} sealed",
		Result = nameof(DescribeSealedType),
		Summary = "Verifies that the <see cref=\"Type\" /> is sealed.",
		NegatedSummary = "Verifies that the <see cref=\"Type\" /> is not sealed.",
		Remarks = """
		          Static types are not considered sealed, even though they
		          have <see cref="Type.IsSealed" /> set to <see langword="true" />.
		          """,
		NegatedRemarks = """
		                 Static types are considered not sealed, even though they
		                 have <see cref="Type.IsSealed" /> set to <see langword="true" />.
		                 """)]
	private static bool IsSealedType(Type type) => type.IsReallySealed();

	private static string DescribeSealedType(Type type)
		=> ResultTexts.Describe(type.IsReallySealed(), "was sealed", "was non-sealed", type);
}
