#if NET8_0_OR_GREATER
using System.Collections.Generic;
using System.Threading;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatAsyncEnumerable
{
	public sealed partial class HasCount
	{
		public sealed class GreaterThanOrEqualTo
		{
			public sealed class Tests
			{
				[Test]
				public async Task ConsidersCancellationToken()
				{
					using CancellationTokenSource cts = new();
					CancellationToken token = cts.Token;
					IAsyncEnumerable<int> subject =
						GetCancellingAsyncEnumerable(4, cts, CancellationToken.None);

					async Task Act()
						=> await That(subject).HasCount().GreaterThanOrEqualTo(6)
							.WithCancellation(token);

					await That(Act).Throws<InconclusiveTestException>()
						.WithMessage("""
						             Expected that subject
						             has at least 6 items,
						             but it could not be verified, because the evaluation was already canceled

						             Collection:
						             [0, 1, 2, 3, (… and maybe more)]
						             """);
				}

				[Test]
				public async Task WhenEnumerableContainsMatchingItems_ShouldSucceed()
				{
					IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3);

					async Task Act()
						=> await That(subject).HasCount().GreaterThanOrEqualTo(3);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenEnumerableContainsTooFewItems_ShouldFail()
				{
					IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3);

					async Task Act()
						=> await That(subject).HasCount().GreaterThanOrEqualTo(4);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             has at least 4 items,
						             but it had only 3 items

						             Collection:
						             [1, 2, 3]
						             """);
				}

				[Test]
				public async Task WhenEnumerableContainsTooManyItems_ShouldSucceed()
				{
					IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3);

					async Task Act()
						=> await That(subject).HasCount().GreaterThanOrEqualTo(2);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenExpectedIsNull_ShouldFail()
				{
					IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3);

					async Task Act()
						=> await That(subject).HasCount().GreaterThanOrEqualTo(null);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             has at least <null> items,
						             but it had at least 1 item

						             Collection:
						             [1, (… and maybe more)]
						             """)
						.Because("nothing can be ordered against null");
				}

				[Test]
				public async Task WhenSourceThrowsAfterEnoughItems_ShouldSucceed()
				{
					IAsyncEnumerable<int> subject =
						ThrowAfter(new InvalidOperationException("enumerated too far"), 1, 1);

					async Task Act()
						=> await That(subject).HasCount().GreaterThanOrEqualTo(1);

					await That(Act).DoesNotThrow()
						.Because("the first item already decides that the collection has at least one item");
				}

				[Test]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					IAsyncEnumerable<int>? subject = null;

					async Task Act()
						=> await That(subject).HasCount().GreaterThanOrEqualTo(2);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             has at least 2 items,
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
							it.HasCount().GreaterThanOrEqualTo(3));

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             does not have at least 3 items,
						             but it had at least 3 items

						             Collection:
						             [1, 2, 3, (… and maybe more)]
						             """);
				}

				[Test]
				public async Task WhenEnumerableContainsTooFewItems_ShouldSucceed()
				{
					IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it =>
							it.HasCount().GreaterThanOrEqualTo(4));

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenExpectedIsNull_ShouldFail()
				{
					IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it =>
							it.HasCount().GreaterThanOrEqualTo(null));

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             does not have at least <null> items,
						             but it had at least 1 item

						             Collection:
						             [1, (… and maybe more)]
						             """)
						.Because("nothing can be ordered against null, so the negation fails as well");
				}
			}
		}
	}
}
#endif
