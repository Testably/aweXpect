using aweXpect.SourceGenerators;

namespace aweXpect;

[CreateExpectationOnNullable<string>("Is{Not}NullOrWhiteSpace", "string.IsNullOrWhiteSpace({value})",
	ExpectationText = "is {not} null or whitespace",
	Summary = "Verifies that the subject is <see langword=\"null\" />, <see cref=\"string.Empty\" /> or consists only of white-space characters.",
	NegatedSummary = "Verifies that the subject is neither <see langword=\"null\" /> nor <see cref=\"string.Empty\" /> and does not consist only of white-space characters.",
	FailOnNull = false,
	NegatedFailsOnNull = true
)]
public static partial class ThatString;
