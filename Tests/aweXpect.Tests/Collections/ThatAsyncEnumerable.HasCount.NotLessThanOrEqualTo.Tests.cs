#if NET8_0_OR_GREATER
using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatAsyncEnumerable
{
	public sealed partial class HasCount
	{
		public sealed class NotLessThanOrEqualTo
		{
			public sealed class Tests
			{
				[Fact]
				public async Task WhenEnumerableContainsMatchingItems_ShouldFail()
				{
					IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3);

					async Task Act()
						=> await That(subject).HasCount().NotLessThanOrEqualTo(3);

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
						=> await That(subject).HasCount().NotLessThanOrEqualTo(2);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenExpectedIsNull_ShouldFail()
				{
					IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3);

					async Task Act()
						=> await That(subject).HasCount().NotLessThanOrEqualTo(null);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             does not have at most <null> items,
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
						=> await That(subject).HasCount().NotLessThanOrEqualTo(2);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             does not have at most 2 items,
						             but it was <null>
						             """);
				}
			}

			public sealed class NegatedTests
			{
				[Fact]
				public async Task WhenEnumerableContainsTooManyItems_ShouldFail()
				{
					IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it =>
							it.HasCount().NotLessThanOrEqualTo(2));

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
				public async Task WhenExpectedIsNull_ShouldFail()
				{
					IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it =>
							it.HasCount().NotLessThanOrEqualTo(null));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has at most <null> items,
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
