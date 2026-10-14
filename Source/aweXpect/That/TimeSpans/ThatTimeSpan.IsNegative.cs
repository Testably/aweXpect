using System;
using aweXpect.Core;
using aweXpect.Core.Extending;

namespace aweXpect;

public static partial class ThatTimeSpan
{
	[GenerateExpectation("Is{Not}Negative", "is {not} negative",
		NullableOverload = false,
		Remarks = """
		          <see cref="TimeSpan.Zero" /> is neither positive nor negative, so it fails
		          <see cref="IsPositive(IThat{TimeSpan})" /> and <see cref="IsNegative(IThat{TimeSpan})" /> and satisfies
		          <see cref="IsNotPositive(IThat{TimeSpan})" /> and <see cref="IsNotNegative(IThat{TimeSpan})" />.
		          """,
		NegatedRemarks = """
		                 <see cref="TimeSpan.Zero" /> is neither positive nor negative, so it fails
		                 <see cref="IsPositive(IThat{TimeSpan})" /> and <see cref="IsNegative(IThat{TimeSpan})" /> and satisfies
		                 <see cref="IsNotPositive(IThat{TimeSpan})" /> and <see cref="IsNotNegative(IThat{TimeSpan})" />.
		                 """)]
	private static bool IsNegativeDuration(TimeSpan value) => value < TimeSpan.Zero;
}
