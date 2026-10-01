#if NET8_0_OR_GREATER
using System.Collections.Generic;
using System.Linq.Expressions;
using aweXpect.Core;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatAsyncEnumerable
{
	public sealed partial class Contains
	{
		public sealed class SingleEnumerationTests
		{
			[Fact]
			public async Task Expectations_WhenExpectedCanOnlyBeEnumeratedOnce_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable([1, 2, 3,]);
				IEnumerable<Action<IThat<int>>> expected =
					Factory.GetSingleUseEnumerable<Action<IThat<int>>>(a => a.IsEqualTo(2), a => a.IsEqualTo(4));

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order and contiguous,
					             but it
					               contained item 3 at index 2 instead of an item that is equal to 4 and
					               lacked 1 of 2 expected items: an item that is equal to 4

					             Collection:
					             [1, 2, 3]

					             Expected:
					             [an item that is equal to 2, an item that is equal to 4]
					             """)
					.Because("the guard, the comparison and the message share one enumeration of the expected items");
			}

			[Fact]
			public async Task Predicates_WhenExpectedCanOnlyBeEnumeratedOnce_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable([1, 2, 3,]);
				IEnumerable<Expression<Func<int, bool>>> expected =
					Factory.GetSingleUseEnumerable<Expression<Func<int, bool>>>(a => a == 2, a => a == 4);

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order and contiguous,
					             but it
					               contained item 3 at index 2 instead of a => (a == 4) and
					               lacked 1 of 2 expected items: a => (a == 4)

					             Collection:
					             [1, 2, 3]

					             Expected:
					             [
					               a => (a == 2),
					               a => (a == 4)
					             ]
					             """)
					.Because("the guard, the comparison and the message share one enumeration of the expected items");
			}

			[Fact]
			public async Task Strings_WhenExpectedCanOnlyBeEnumeratedOnce_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
				IEnumerable<string> expected = Factory.GetSingleUseEnumerable("b", "d");

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order and contiguous,
					             but it
					               contained item "c" at index 2 instead of "d" and
					               lacked 1 of 2 expected items: "d"

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               "b",
					               "d"
					             ]
					             """)
					.Because("the guard, the comparison and the message share one enumeration of the expected items");
			}

			[Fact]
			public async Task WhenExpectedCanOnlyBeEnumeratedOnce_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable([1, 2, 3,]);
				IEnumerable<int> expected = Factory.GetSingleUseEnumerable(2, 4);

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order and contiguous,
					             but it
					               contained item 3 at index 2 instead of 4 and
					               lacked 1 of 2 expected items: 4

					             Collection:
					             [1, 2, 3]

					             Expected:
					             [2, 4]
					             """)
					.Because("the guard, the comparison and the message share one enumeration of the expected items");
			}

			[Fact]
			public async Task WhenUnexpectedCanOnlyBeEnumeratedOnce_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable([1, 2, 3,]);
				IEnumerable<int> unexpected = Factory.GetSingleUseEnumerable(2, 3);

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection unexpected in order and contiguous,
					             but it did

					             Collection:
					             [1, 2, 3, (… and maybe more)]

					             Expected:
					             [2, 3]
					             """)
					.Because("the guard, the comparison and the message share one enumeration of the unexpected items");
			}

			[Fact]
			public async Task Within_WhenExpectedCanOnlyBeEnumeratedOnce_ShouldFail()
			{
				IAsyncEnumerable<double> subject = ToAsyncEnumerable(1.0, 2.0, 3.0);
				IEnumerable<double> expected = Factory.GetSingleUseEnumerable(2.1, 4.0);

				async Task Act()
					=> await That(subject).Contains(expected).Within(0.25);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected ± 0.25 in order and contiguous,
					             but it
					               contained item 3.0 at index 2 instead of 4.0 and
					               lacked 1 of 2 expected items: 4.0

					             Collection:
					             [1.0, 2.0, 3.0]

					             Expected:
					             [2.1, 4.0]
					             """)
					.Because("the guard, the comparison and the message share one enumeration of the expected items");
			}
		}
	}
}
#endif
