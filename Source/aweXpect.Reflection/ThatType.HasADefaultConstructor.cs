using System;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatType
{
	[GenerateExpectation("HasADefaultConstructor", "has a default constructor",
		NegatedName = "DoesNotHaveADefaultConstructor",
		NegatedExpectationText = "does not have a default constructor",
		Result = nameof(DescribeDefaultConstructor),
		Summary = "Verifies that the <see cref=\"Type\" /> has an accessible parameterless (default) constructor.",
		NegatedSummary = "Verifies that the <see cref=\"Type\" /> does not have an accessible parameterless (default) constructor.")]
	private static bool HasADefaultConstructorType(Type type) => type.HasDefaultConstructor();

	private static string DescribeDefaultConstructor(Type type)
		=> ResultTexts.Describe(type.HasDefaultConstructor(),
			"had a default constructor", "did not have a default constructor", type);
}
