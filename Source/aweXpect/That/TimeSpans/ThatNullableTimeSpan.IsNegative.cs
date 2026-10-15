using System;
using System.Diagnostics.CodeAnalysis;
using aweXpect.Core.Extending;

namespace aweXpect;

public static partial class ThatNullableTimeSpan
{
	[GenerateExpectation("Is{Not}Negative", "is {not} negative",
		Remarks = """
		          <see cref="TimeSpan.Zero" /> is neither positive nor negative, so it fails
		          <see cref="IsPositive(aweXpect.Core.IThat{TimeSpan?})" /> and <see cref="IsNegative(aweXpect.Core.IThat{TimeSpan?})" /> and satisfies
		          <see cref="IsNotPositive(aweXpect.Core.IThat{TimeSpan?})" /> and <see cref="IsNotNegative(aweXpect.Core.IThat{TimeSpan?})" />.
		          """,
		NegatedRemarks = """
		                 <see cref="TimeSpan.Zero" /> is neither positive nor negative, so it fails
		                 <see cref="IsPositive(aweXpect.Core.IThat{TimeSpan?})" /> and <see cref="IsNegative(aweXpect.Core.IThat{TimeSpan?})" /> and satisfies
		                 <see cref="IsNotPositive(aweXpect.Core.IThat{TimeSpan?})" /> and <see cref="IsNotNegative(aweXpect.Core.IThat{TimeSpan?})" />.<br />
		                 <see langword="null" /> is neither treated as negative nor as not negative, so it fails.
		                 """)]
	private static bool IsNegativeDuration([DisallowNull] TimeSpan? value) => value.Value < TimeSpan.Zero;
}
