using System;
using System.Diagnostics.CodeAnalysis;
using aweXpect.Core;
using aweXpect.Core.Extending;

namespace aweXpect;

public static partial class ThatNullableTimeSpan
{
	[GenerateExpectation("Is{Not}Negative", "is {not} negative",
		Remarks = """
		          <see cref="TimeSpan.Zero" /> is neither positive nor negative, so it fails
		          <see cref="IsPositive(IThat{TimeSpan?})" /> and <see cref="IsNegative(IThat{TimeSpan?})" /> and satisfies
		          <see cref="IsNotPositive(IThat{TimeSpan?})" /> and <see cref="IsNotNegative(IThat{TimeSpan?})" />.
		          """,
		NegatedRemarks = """
		                 <see cref="TimeSpan.Zero" /> is neither positive nor negative, so it fails
		                 <see cref="IsPositive(IThat{TimeSpan?})" /> and <see cref="IsNegative(IThat{TimeSpan?})" /> and satisfies
		                 <see cref="IsNotPositive(IThat{TimeSpan?})" /> and <see cref="IsNotNegative(IThat{TimeSpan?})" />.<br />
		                 <see langword="null" /> is neither treated as negative nor as not negative, so it fails.
		                 """)]
	private static bool IsNegativeDuration([DisallowNull] TimeSpan? value) => value.Value < TimeSpan.Zero;
}
