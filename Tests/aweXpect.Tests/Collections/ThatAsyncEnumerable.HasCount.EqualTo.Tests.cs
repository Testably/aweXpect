#if NET8_0_OR_GREATER
using System.Collections.Generic;
using System.Threading;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatAsyncEnumerable
{
	public sealed partial class HasCount
	{
		public sealed class EqualTo
		{
			public sealed class Tests
			{
				[Test]
				public async Task ConsidersCancellationToken()
				{
					using CancellationTokenSource cts = new();
					CancellationToken token = cts.Token;
					IAsyncEnumerable<int> subject =
						GetCancellingAsyncEnumerable(6, cts, CancellationToken.None);

					async Task Act()
						=> await That(subject).HasCount().EqualTo(6)
							.WithCancellation(token);

					await That(Act).Throws<InconclusiveTestException>()
						.WithMessage("""
						             Expected that subject
						             has exactly 6 items,
						             but it could not be verified, because the evaluation was already canceled

						             Collection:
						             [0, 1, 2, 3, 4, 5, (… and maybe more)]
						             """);
				}

				[Test]
				public async Task WhenAnEarlierAttemptReadAllItems_ShouldNotReuseItsTotalCount()
				{
					int calls = 0;
					Func<IAsyncEnumerable<int>> subject =
						() => ++calls == 1 ? ToAsyncEnumerable(1) : ToAsyncEnumerable(1, 2, 3, 4);

					async Task Act()
						=> await That(subject).Eventually().WithinTwoAttempts(5.Seconds())
							.HasCount().EqualTo(2);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             eventually has exactly 2 items within 0:05,
						             but it had at least 3 items

						             Collection:
						             [1, 2, 3, (… and maybe more)]
						             """);
				}

				[Test]
				public async Task WhenEnumerableContainsMatchingItems_ShouldSucceed()
				{
					IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3);

					async Task Act()
						=> await That(subject).HasCount().EqualTo(3);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenEnumerableContainsTooFewItems_ShouldFail()
				{
					IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3);

					async Task Act()
						=> await That(subject).HasCount().EqualTo(4);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             has exactly 4 items,
						             but it had only 3 items

						             Collection:
						             [1, 2, 3]
						             """);
				}

				[Test]
				public async Task WhenEnumerableContainsTooManyItems_ShouldFail()
				{
					IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3);

					async Task Act()
						=> await That(subject).HasCount().EqualTo(2);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             has exactly 2 items,
						             but it had at least 3 items

						             Collection:
						             [1, 2, 3, (… and maybe more)]
						             """);
				}

				[Test]
				public async Task WhenExpectedIsNull_ShouldFail()
				{
					IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3);

					async Task Act()
						=> await That(subject).HasCount().EqualTo(null);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             has exactly <null> items,
						             but it had at least 1 item

						             Collection:
						             [1, (… and maybe more)]
						             """)
						.Because("a count is never null, so the enumeration can stop at the first item");
				}

				[Test]
				public async Task WhenSubjectIsNull_AndExpectedIsNull_ShouldFail()
				{
					IAsyncEnumerable<int>? subject = null;

					async Task Act()
						=> await That(subject).HasCount().EqualTo(null);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             has exactly <null> items,
						             but it was <null>
						             """);
				}

				[Test]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					IAsyncEnumerable<int>? subject = null;

					async Task Act()
						=> await That(subject).HasCount().EqualTo(2);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             has exactly 2 items,
						             but it was <null>
						             """);
				}
			}

			public sealed class NegatedTests
			{
				[Test]
				public async Task WhenEnumerableContainsMatchingItems_ShouldFail()
				{
					IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it =>
							it.HasCount().EqualTo(3));

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             does not have exactly 3 items,
						             but it had 3 items

						             Collection:
						             [1, 2, 3]
						             """);
				}

				[Test]
				public async Task WhenEnumerableContainsTooManyItems_ShouldSucceed()
				{
					IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it =>
							it.HasCount().EqualTo(2));

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenExpectedIsNull_ShouldSucceed()
				{
					IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it =>
							it.HasCount().EqualTo(null));

					await That(Act).DoesNotThrow()
						.Because("a count is never null");
				}
			}
		}
	}
}
#endif
