#if NET8_0_OR_GREATER
using System.Collections.Generic;
using System.Threading;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatAsyncEnumerable
{
	public sealed partial class HasCount
	{
		public sealed class LessThanOrEqualTo
		{
			public sealed class Tests
			{
				[Fact]
				public async Task ConsidersCancellationToken()
				{
					using CancellationTokenSource cts = new();
					CancellationToken token = cts.Token;
					IAsyncEnumerable<int> subject =
						GetCancellingAsyncEnumerable(6, cts, CancellationToken.None);

					async Task Act()
						=> await That(subject).HasCount().LessThanOrEqualTo(6)
							.WithCancellation(token);

					await That(Act).Throws<InconclusiveException>()
						.WithMessage("""
						             Expected that subject
						             has at most 6 items,
						             but it could not be verified, because the evaluation was already canceled

						             Collection:
						             [0, 1, 2, 3, 4, 5, (… and maybe more)]
						             """);
				}

				[Fact]
				public async Task WhenEnumerableContainsMatchingItems_ShouldSucceed()
				{
					IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3);

					async Task Act()
						=> await That(subject).HasCount().LessThanOrEqualTo(3);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenEnumerableContainsTooFewItems_ShouldSucceed()
				{
					IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3);

					async Task Act()
						=> await That(subject).HasCount().LessThanOrEqualTo(4);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenEnumerableContainsTooManyItems_ShouldFail()
				{
					IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3);

					async Task Act()
						=> await That(subject).HasCount().LessThanOrEqualTo(2);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has at most 2 items,
						             but it had at least 3 items

						             Collection:
						             [1, 2, 3, (… and maybe more)]
						             """);
				}

				[Fact]
				public async Task WhenEvaluatedForSeveralItems_ShouldJudgeEachItemOnItsOwn()
				{
					IAsyncEnumerable<int>[] subject = [ToAsyncEnumerable(1), ToAsyncEnumerable(1, 2, 3, 4),];

					async Task Act()
						=> await That(subject).All().ComplyWith(x => x.HasCount().LessThanOrEqualTo(2));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has at most 2 items for all items,
						             but only 1 of 2 did

						             Not matching items:
						             [
						               IAsyncEnumerable<int>
						             ]

						             Collection:
						             [
						               IAsyncEnumerable<int>,
						               IAsyncEnumerable<int>
						             ]

						             Collection (item [1]):
						             [1, 2, 3, (… and maybe more)]
						             """);
				}

				[Fact]
				public async Task WhenExpectedIsNull_ShouldFail()
				{
					IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3);

					async Task Act()
						=> await That(subject).HasCount().LessThanOrEqualTo(null);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has at most <null> items,
						             but it had at least 1 item

						             Collection:
						             [1, (… and maybe more)]
						             """)
						.Because("nothing can be ordered against null");
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					IAsyncEnumerable<int>? subject = null;

					async Task Act()
						=> await That(subject).HasCount().LessThanOrEqualTo(2);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has at most 2 items,
						             but it was <null>
						             """);
				}
			}

			public sealed class NegatedTests
			{
				[Fact]
				public async Task WhenEnumerableContainsMatchingItems_ShouldFail()
				{
					IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it =>
							it.HasCount().LessThanOrEqualTo(3));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             does not have at most 3 items,
						             but it had 3 items

						             Collection:
						             [1, 2, 3]
						             """);
				}

				[Fact]
				public async Task WhenEnumerableContainsTooManyItems_ShouldSucceed()
				{
					IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it =>
							it.HasCount().LessThanOrEqualTo(2));

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenExpectedIsNull_ShouldFail()
				{
					IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it =>
							it.HasCount().LessThanOrEqualTo(null));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             does not have at most <null> items,
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
