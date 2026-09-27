using aweXpect.SourceGenerators;

namespace aweXpect;

[CreateExpectationOnNullable<string>("Is{Not}NullOrEmpty", "string.IsNullOrEmpty({value})",
	ExpectationText = "is {not} null or empty",
	Summary = "Verifies that the subject is <see langword=\"null\" /> or <see cref=\"string.Empty\" />.",
	NegatedSummary = "Verifies that the subject is neither <see langword=\"null\" /> nor <see cref=\"string.Empty\" />.",
	FailOnNull = false,
	NegatedFailsOnNull = true
)]
public static partial class ThatString;
