using System;
using aweXpect.Core.Extending;

namespace aweXpect;

public static partial class ThatTimeSpan
{
	[GenerateExpectation("Is{Not}Positive", "is {not} positive",
		NullableOverload = false,
		Remarks = """
		          <see cref="TimeSpan.Zero" /> is neither positive nor negative, so it fails
		          <see cref="IsPositive(aweXpect.Core.IThat{TimeSpan})" /> and <see cref="IsNegative(aweXpect.Core.IThat{TimeSpan})" /> and satisfies
		          <see cref="IsNotPositive(aweXpect.Core.IThat{TimeSpan})" /> and <see cref="IsNotNegative(aweXpect.Core.IThat{TimeSpan})" />.
		          """,
		NegatedRemarks = """
		                 <see cref="TimeSpan.Zero" /> is neither positive nor negative, so it fails
		                 <see cref="IsPositive(aweXpect.Core.IThat{TimeSpan})" /> and <see cref="IsNegative(aweXpect.Core.IThat{TimeSpan})" /> and satisfies
		                 <see cref="IsNotPositive(aweXpect.Core.IThat{TimeSpan})" /> and <see cref="IsNotNegative(aweXpect.Core.IThat{TimeSpan})" />.
		                 """)]
	private static bool IsPositiveDuration(TimeSpan value) => value > TimeSpan.Zero;
}
