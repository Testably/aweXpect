using System;
using aweXpect.SourceGenerators;

namespace aweXpect;

[CreateExpectationOnNullable<Guid>("Is{Not}NullOrEmpty", "{value} is null || {value} == Guid.Empty",
	ExpectationText = "is {not} null or empty",
	NegatedSummary = "Verifies that the subject is neither <see langword=\"null\" /> nor <see cref=\"Guid.Empty\" />.",
	Using = ["System",],
	FailOnNull = false,
	NegatedFailsOnNull = true
)]
public static partial class ThatNullableGuid;
