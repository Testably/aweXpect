using System;
using System.Diagnostics.CodeAnalysis;
using aweXpect.Core.Extending;

namespace aweXpect;

public static partial class ThatNullableGuid
{
	[GenerateExpectation("Is{Not}NullOrEmpty", "is {not} null or empty",
		NegatedSummary = "Verifies that the subject is neither <see langword=\"null\" /> nor <see cref=\"Guid.Empty\" />.")]
	private static bool IsNullOrEmptyGuid([NotNullWhen(false)] Guid? value) => value is null || value == Guid.Empty;
}
