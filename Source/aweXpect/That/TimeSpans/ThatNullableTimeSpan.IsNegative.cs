using System;
using aweXpect.SourceGenerators;

namespace aweXpect;

[CreateExpectationOnNullable<TimeSpan>("Is{Not}Negative", "{value} < TimeSpan.Zero",
	ExpectationText = "is {not} negative",
	Remarks = """
	          <see cref="TimeSpan.Zero" /> is neither positive nor negative, so it fails
	          <see cref="IsPositive(IThat{TimeSpan?})" /> and <see cref="IsNegative(IThat{TimeSpan?})" /> and satisfies
	          <see cref="IsNotPositive(IThat{TimeSpan?})" /> and <see cref="IsNotNegative(IThat{TimeSpan?})" />.
	          """,
	Using = ["System",]
)]
public static partial class ThatNullableTimeSpan;
