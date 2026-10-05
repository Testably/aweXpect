using aweXpect.SourceGenerators;

namespace aweXpect;

[CreateExpectationOnNullable<string>("Is{Not}Empty", "{value} == string.Empty",
	ExpectationText = "is {not} empty",
	NegatedRemarks = "<see langword=\"null\" /> is neither treated as empty nor as not empty, so it fails."
)]
public static partial class ThatString;
