using System;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatType
{
	[GenerateExpectation("Is{Not}Abstract", "is {not} abstract",
		Result = nameof(DescribeAbstractType),
		Summary = "Verifies that the <see cref=\"Type\" /> is abstract.",
		NegatedSummary = "Verifies that the <see cref=\"Type\" /> is not abstract.",
		Remarks = """
		          Static types or interfaces are not considered abstract, even though they
		          have <see cref="Type.IsAbstract" /> set to <see langword="true" />.
		          """,
		NegatedRemarks = """
		                 Static types or interfaces are considered not abstract, even though they
		                 have <see cref="Type.IsAbstract" /> set to <see langword="true" />.
		                 """)]
	private static bool IsAbstractType(Type type) => type.IsReallyAbstract();

	private static string DescribeAbstractType(Type type)
		=> ResultTexts.Describe(type.IsReallyAbstract(), "was abstract", "was non-abstract", type);
}
