using System;
using System.Diagnostics.CodeAnalysis;
using aweXpect.Core.Extending;

namespace aweXpect;

public static partial class ThatNullableTimeSpan
{
	[GenerateExpectation("Is{Not}Positive", "is {not} positive",
		Remarks = """
		          <see cref="TimeSpan.Zero" /> is neither positive nor negative, so it fails
		          <see cref="IsPositive(aweXpect.Core.IThat{TimeSpan?})" /> and <see cref="IsNegative(aweXpect.Core.IThat{TimeSpan?})" /> and satisfies
		          <see cref="IsNotPositive(aweXpect.Core.IThat{TimeSpan?})" /> and <see cref="IsNotNegative(aweXpect.Core.IThat{TimeSpan?})" />.
		          """,
		NegatedRemarks = """
		                 <see cref="TimeSpan.Zero" /> is neither positive nor negative, so it fails
		                 <see cref="IsPositive(aweXpect.Core.IThat{TimeSpan?})" /> and <see cref="IsNegative(aweXpect.Core.IThat{TimeSpan?})" /> and satisfies
		                 <see cref="IsNotPositive(aweXpect.Core.IThat{TimeSpan?})" /> and <see cref="IsNotNegative(aweXpect.Core.IThat{TimeSpan?})" />.<br />
		                 <see langword="null" /> is neither treated as positive nor as not positive, so it fails.
		                 """)]
	private static bool IsPositiveDuration([DisallowNull] TimeSpan? value) => value.Value > TimeSpan.Zero;
}
