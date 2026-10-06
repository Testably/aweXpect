#if NET8_0_OR_GREATER
using System.Collections.Generic;

namespace aweXpect.Tests;

public sealed partial class ThatAsyncEnumerable
{
	public sealed class Any
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenAtLeastOneItemMatches_ShouldSucceed()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3, 4, 5);

				async Task Act()
					=> await That(subject).Any().ComplyWith(it => it.IsEqualTo(3));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenBoolItemsDoNotMatch_ShouldRenderThemOnOneLine()
			{
				IAsyncEnumerable<bool> subject = ToAsyncEnumerable(false, false);

				async Task Act()
					=> await That(subject).Any().ComplyWith(it => it.IsEqualTo(true));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is True for at least one item,
					             but none of 2 were

					             Collection:
					             [False, False]
					             """);
			}

			[Test]
			public async Task WhenFirstItemCompliesAndSourceHangs_ShouldNotWaitForMoreItems()
			{
				IAsyncEnumerable<int> subject = HangAfter([1,]);

				async Task Act()
					=> await That(subject).Any().ComplyWith(it => it.IsEqualTo(1)).WithTimeout(30.Seconds());

				await That(Act).ExecutesIn().AtMost(10.Seconds())
					.Because("the first item already decides that at least one item complies");
			}

			[Test]
			public async Task WhenFirstItemSatisfiesAndSourceHangs_ShouldNotWaitForMoreItems()
			{
				IAsyncEnumerable<int> subject = HangAfter([1,]);

				async Task Act()
					=> await That(subject).Any().Satisfy(x => x == 1).WithTimeout(30.Seconds());

				await That(Act).ExecutesIn().AtMost(10.Seconds())
					.Because("the first item already decides that at least one item satisfies the predicate");
			}

			[Test]
			public async Task WhenItemsDoNotComplyWithAndSubjectIsNull_ShouldNegateExpectation()
			{
				IAsyncEnumerable<int>? subject = null;

				async Task Act()
					=> await That(subject).Any()
						.ComplyWith(it => it.DoesNotComplyWith(x => x.IsEqualTo(1).Or.IsEqualTo(2)));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to 1 and is not equal to 2 for at least one item,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenNestedItemsDoNotComplyWithAndSubjectIsNull_ShouldNegateExpectation()
			{
				IAsyncEnumerable<IAsyncEnumerable<int>>? subject = null;

				async Task Act()
					=> await That(subject).Any()
						.ComplyWith(it => it.Any().ComplyWith(x => x.DoesNotComplyWith(y => y.IsEqualTo(1).Or.IsEqualTo(2))));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to 1 and is not equal to 2 for at least one item for at least one item,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenNoItemsMatch_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3);

				async Task Act()
					=> await That(subject).Any().ComplyWith(it => it.IsEqualTo(99));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 99 for at least one item,
					             but none of 3 were

					             Collection:
					             [1, 2, 3]
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_NegatedShouldFail()
			{
				IAsyncEnumerable<int>? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.Any().ComplyWith(x => x.IsEqualTo(1)));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 1 for no items,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IAsyncEnumerable<int>? subject = null;

				async Task Act()
					=> await That(subject).Any().ComplyWith(it => it.IsEqualTo(1));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 1 for at least one item,
					             but it was <null>
					             """);
			}
		}

		public sealed class StringTests
		{
			[Test]
			public async Task WhenAtLeastOneItemMatches_ShouldSucceed()
			{
				IAsyncEnumerable<string?> subject = ToAsyncEnumerable<string?>("apple", "banana", "cherry");

				async Task Act()
					=> await That(subject).Any().ComplyWith(it => it.StartsWith("b"));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenNoItemsMatch_ShouldFail()
			{
				IAsyncEnumerable<string?> subject = ToAsyncEnumerable<string?>("apple", "cherry");

				async Task Act()
					=> await That(subject).Any().ComplyWith(it => it.StartsWith("b"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             starts with "b" for at least one item,
					             but none of 2 did

					             Collection:
					             [
					               "apple",
					               "cherry"
					             ]
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenAnyItemComplies_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3, 4, 5);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.Any().ComplyWith(x => x.IsGreaterThan(2)));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is greater than 2 for no items,
					             but at least 1 of at least 3 were

					             Matching items:
					             [3, (… and maybe more)]

					             Collection:
					             [1, 2, 3, (… and maybe more)]
					             """);
			}
		}
	}
}
#endif
