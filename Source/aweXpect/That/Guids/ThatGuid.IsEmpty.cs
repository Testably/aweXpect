using System;
using aweXpect.SourceGenerators;

namespace aweXpect;

[CreateExpectationOn<Guid>("Is{Not}Empty", "{value} == Guid.Empty",
	ExpectationText = "is {not} empty",
	Using = ["System",]
)]
public static partial class ThatGuid;
