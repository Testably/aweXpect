using aweXpect.SourceGenerators;

namespace aweXpect;

[CreateExpectationOnNullable<string>("Is{Not}Empty", "{value} == string.Empty",
	ExpectationText = "is {not} empty"
)]
public static partial class ThatString;
