using System;
using aweXpect.SourceGenerators;

namespace aweXpect;

[CreateExpectationOnNullable<Guid>("Is{Not}Empty", "{value} == Guid.Empty",
	ExpectationText = "is {not} empty",
	NegatedRemarks = "<see langword=\"null\" /> is neither treated as empty nor as not empty, so it fails.",
	Using = ["System",]
)]
public static partial class ThatNullableGuid;
