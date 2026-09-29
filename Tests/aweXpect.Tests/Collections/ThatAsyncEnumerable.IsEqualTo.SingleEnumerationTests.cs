#if NET8_0_OR_GREATER
using System.Collections.Generic;
using System.Linq.Expressions;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatAsyncEnumerable
{
	public sealed partial class IsEqualTo
	{
		public sealed class SingleEnumerationTests
		{
			[Fact]
			public async Task Predicates_WhenExpectedCanOnlyBeEnumeratedOnce_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable([1, 3, 2,]);
				IEnumerable<Expression<Func<int, bool>>> expected =
					Factory.GetSingleUseEnumerable<Expression<Func<int, bool>>>(a => a == 1, a => a == 2, a => a == 3);

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order,
					             but it
					               contained item 3 at index 1 instead of a => (a == 2) and
					               contained item 2 at index 2 instead of a => (a == 3)
					             (but the items match in a different order)

					             Collection:
					             [1, 3, 2]

					             Expected:
					             [
					               a => (a == 1),
					               a => (a == 2),
					               a => (a == 3)
					             ]
					             """)
					.Because("the comparison, the hint and the message share one enumeration of the expected items");
			}

			[Fact]
			public async Task WhenExpectedCanOnlyBeEnumeratedOnce_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable([1, 3, 2,]);
				IEnumerable<int> expected = Factory.GetSingleUseEnumerable(1, 2, 3);

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order,
					             but it
					               contained item 3 at index 1 instead of 2 and
					               contained item 2 at index 2 instead of 3
					             (but the items match in a different order)

					             Collection:
					             [1, 3, 2]

					             Expected:
					             [
					               1,
					               2,
					               3
					             ]
					             """)
					.Because("the comparison, the hint and the message share one enumeration of the expected items");
			}
		}
	}
}
#endif
